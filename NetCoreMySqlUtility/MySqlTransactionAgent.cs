using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace NetCoreMySqlUtility
{
    /// <summary>
    /// 表示统一调用SQL事务对象的代理类
    /// <para>使用方式一：var agent = MySqlTransactionAgent.Init(conn);agent.AddBatchSteps(steps);agent.StartBatchStepsAndCommitAtOnce();</para>
    /// <para>使用方式二：var agent = MySqlTransactionAgent.Init(conn);agent.StartSingleStep(step1);agent.StartSingleStep(step2);agent.CommitStepsAtLast();</para>
    /// </summary>
    public class MySqlTransactionAgent
    {
        #region 委托和事件
        /// <summary>
        /// 在事务步骤执行成功时的委托行为
        /// </summary>
        /// <param name="r">结果</param>
        public delegate void AfterStepSucceedAction(TransactionStep currentStep);

        /// <summary>
        /// 在事务步骤执行失败时的委托行为
        /// </summary>
        /// <param name="r">结果</param>
        public delegate void AfterStepFailedAction(TransactionStep currentStep, SqlTransactionError ex);

        /// <summary>
        /// 事务提交成功时的委托行为
        /// </summary>
        /// <param name="r"></param>
        public delegate void AfterCommitSucceedAction(List<TransactionStep> steps);

        /// <summary>
        /// 事务提交失败时的委托行为
        /// </summary>
        /// <param name="r"></param>
        public delegate void AfterCommitFailedAction(SqlTransactionError ex);

        #region 事件
        /// <summary>
        /// 当前步骤执行成功的事件处理，此时当前步骤已经完成，事务尚未提交
        /// </summary>
        public event AfterStepSucceedAction OnStepSucceed;

        /// <summary>
        /// 当前步骤执行失败的事件处理，此时事务已经回滚
        /// </summary>
        public event AfterStepFailedAction OnStepFailed;

        /// <summary>
        /// 事务整体提交成功的事件处理，此时事务已经结束
        /// </summary>
        public event AfterCommitSucceedAction OnCommitSucceed;

        /// <summary>
        /// 事务整体提交失败的事件处理，此时事务已经回滚
        /// </summary>
        public event AfterCommitFailedAction OnCommitFailed;
        #endregion

        #endregion

        #region 私有字段
        private MySqlConnection m_Conn;
        private List<TransactionStep> m_Steps;
        private MySqlTransaction m_StepTrans;
        #endregion

        /// <summary>
        /// 使用一个新建的连接实例初始化SQL事务对象的代理器
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static MySqlTransactionAgent Init(MySqlConnection conn)
        {
            return new MySqlTransactionAgent(conn);
        }

        /// <summary>
        /// 使用一个新建的连接实例初始化SQL事务对象的代理器
        /// </summary>
        /// <param name="conn"></param>
        internal MySqlTransactionAgent(MySqlConnection conn)
        {
            if (conn == null) { throw new ArgumentNullException("conn"); }
            m_Conn = conn;
            m_Steps = new List<TransactionStep>();
        }

        /// <summary>
        /// 添加要以顺序批量执行的事务步骤，以待成批执行
        /// </summary>
        /// <param name="step"></param>
        public void AddBatchSteps(TransactionStep[] steps)
        {
            if (steps != null)
            {
                m_Steps.AddRange(steps);
            }
        }

        /// <summary>
        /// 从头开始按顺序批量执行一批事务步骤并一次性提交(先通过AddBatchSteps方法添加事务步骤)
        /// </summary>
        public void StartBatchStepsAndCommitAtOnce()
        {
            MySqlTransaction trans = null;
            TransactionStep currentStep = null;
            SqlTransactionError error = null;

            if (m_Steps == null || m_Steps.Count == 0)
            {
                throw new Exception("请先调用AddTransactionStep添加事务步骤");
            }

            try
            {
                if (m_Conn.State != ConnectionState.Open)
                {
                    m_Conn.Open();
                }

                trans = m_Conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                //按顺序执行步骤
                for (int i = 0; i < m_Steps.Count; i++)
                {
                    currentStep = m_Steps[i];
                    using (var cmd = m_Conn.CreateCommand())
                    {
                        cmd.Transaction = trans;
                        cmd.CommandText = currentStep.SqlCommands;
                        if (currentStep.SqlParamas.Length > 0)
                        {
                            cmd.Parameters.AddRange(currentStep.SqlParamas);
                        }
                        cmd.CommandType = currentStep.CommandType;
                        switch (currentStep.SqlCommandResultType)
                        {
                            case SqlCommandResultType.Effects:
                                currentStep.EffectsResult = cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                cmd.Dispose();
                                break;
                            case SqlCommandResultType.DataTable:
                                MySqlDataAdapter sda = new MySqlDataAdapter();
                                sda.SelectCommand = cmd;
                                DataSet ds = new DataSet();
                                sda.Fill(ds);
                                if (ds.Tables.Count > 0)
                                {
                                    currentStep.DataTableResult = ds.Tables[0];
                                }
                                cmd.Parameters.Clear();
                                cmd.Dispose();
                                break;
                            case SqlCommandResultType.DataSet:
                                MySqlDataAdapter sda2 = new MySqlDataAdapter();
                                sda2.SelectCommand = cmd;
                                DataSet ds2 = new DataSet();
                                sda2.Fill(ds2);
                                currentStep.DataSetResult = ds2;
                                cmd.Parameters.Clear();
                                cmd.Dispose();
                                break;
                        }

                        if (this.OnStepSucceed != null)
                        {
                            this.OnStepSucceed.Invoke(currentStep);
                        }
                    }

                }

                //提交事务
                trans.Commit();

                //事件处理
                if (this.OnCommitSucceed != null)
                {
                    this.OnCommitSucceed.Invoke(m_Steps);
                }

            }
            catch (Exception ex)
            {
                error = new SqlTransactionError("事务处理失败", currentStep, ex);
                //回滚,清理资源
                if (trans != null)
                {
                    trans.Rollback();

                    //事件处理
                    if (this.OnCommitFailed != null)
                    {
                        this.OnCommitFailed.Invoke(error);
                    }
                }
                throw error;
            }
            finally
            {
                if (trans != null)
                {
                    if (m_Conn.State != ConnectionState.Closed)
                    {
                        m_Conn.Close();
                    }
                    trans.Dispose();
                }
            }
        }

        /// <summary>
        /// 清空添加过的事务步骤
        /// </summary>
        public void ClearSteps()
        {
            m_Steps.Clear();
            //string temp = m_Conn.ConnectionString;
            //m_Conn.Dispose();
            //m_Conn = new SqlConnection(temp);
        }

        /// <summary>
        /// 一次执行一步事务中的步骤
        /// </summary>
        /// <param name="currentStep">当前步骤</param>
        /// <returns>返回带有SQL执行结果的步骤</returns>
        public TransactionStep StartSingleStep(TransactionStep currentStep)
        {
            SqlTransactionError error = null;

            try
            {
                if (m_StepTrans == null)
                {
                    if (m_Conn.State != ConnectionState.Open)
                    {
                        m_Conn.Open();
                    }

                    m_StepTrans = m_Conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                }

                using (var cmd = m_Conn.CreateCommand())
                {
                    cmd.Transaction = m_StepTrans;
                    cmd.CommandText = currentStep.SqlCommands;
                    if (currentStep.SqlParamas.Length > 0)
                    {
                        cmd.Parameters.AddRange(currentStep.SqlParamas);
                    }
                    cmd.CommandType = currentStep.CommandType;
                    switch (currentStep.SqlCommandResultType)
                    {
                        case SqlCommandResultType.Effects:
                            currentStep.EffectsResult = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            break;
                        case SqlCommandResultType.DataTable:
                            MySqlDataAdapter sda = new MySqlDataAdapter();
                            sda.SelectCommand = cmd;
                            DataSet ds = new DataSet();
                            sda.Fill(ds);
                            if (ds.Tables.Count > 0)
                            {
                                currentStep.DataTableResult = ds.Tables[0];
                            }
                            cmd.Parameters.Clear();
                            break;
                        case SqlCommandResultType.DataSet:
                            MySqlDataAdapter sda2 = new MySqlDataAdapter();
                            sda2.SelectCommand = cmd;
                            DataSet ds2 = new DataSet();
                            sda2.Fill(ds2);
                            currentStep.DataSetResult = ds2;
                            cmd.Parameters.Clear();
                            break;
                        case SqlCommandResultType.Scalar:
                            currentStep.ScalarResult = cmd.ExecuteScalar();
                            cmd.Parameters.Clear();
                            break;
                    }

                    if (this.OnStepSucceed != null)
                    {
                        this.OnStepSucceed.Invoke(currentStep);
                    }
                }
            }
            catch (Exception ex)
            {
                error = new SqlTransactionError("单步事务处理失败," + currentStep.StepName + "," + ex.Message, currentStep, ex);

                //回滚清理
                if (m_StepTrans != null)
                {
                    m_StepTrans.Rollback();
                    m_StepTrans.Dispose();
                    m_StepTrans = null;
                }

                if (m_Conn.State != ConnectionState.Closed)
                {
                    m_Conn.Close();
                    //m_Conn.Dispose();
                }

                //事件处理
                if (this.OnStepFailed != null)
                {
                    this.OnStepFailed.Invoke(currentStep, error);
                }

                if (this.OnCommitFailed != null)
                {
                    this.OnCommitFailed.Invoke(error);
                }

                throw error;
            }

            return currentStep;
        }

        /// <summary>
        /// 提交一步一步执行的事务（在所有StartSingleStep调用之后执行）
        /// </summary>
        public void CommitStepsAtLast()
        {
            SqlTransactionError error = null;

            if (m_StepTrans == null)
            {
                throw new Exception("没有执行过事务步骤，请先调用StartSingleStep");
            }
            else
            {
                try
                {
                    m_StepTrans.Commit();
                }
                catch (Exception ex)
                {
                    error = new SqlTransactionError("事务提交失败", null, ex);

                    //回滚,清理资源
                    if (m_StepTrans != null)
                    {
                        m_StepTrans.Rollback();

                        //事件处理
                        if (this.OnCommitFailed != null)
                        {
                            this.OnCommitFailed.Invoke(error);
                        }
                    }

                    throw error;
                }
                finally
                {
                    if (m_StepTrans != null)
                    {
                        if (m_Conn.State != ConnectionState.Closed)
                        {
                            m_Conn.Close();
                        }
                        m_StepTrans.Dispose();
                    }
                }
            }
        }

    }

    /// <summary>
    ///表示要列入事务中执行的步骤
    /// </summary>
    public class TransactionStep
    {
        /// <summary>
        /// SQL命令返回的结果类型
        /// </summary>
        public SqlCommandResultType SqlCommandResultType { get; set; } = SqlCommandResultType.Effects;

        /// <summary>
        /// SQL命令类型
        /// </summary>
        public System.Data.CommandType CommandType { get; set; } = CommandType.Text;

        /// <summary>
        /// 当前阶段要执行的SQL
        /// </summary>
        public string SqlCommands { get; set; }

        /// <summary>
        /// 步骤名称
        /// </summary>
        public string StepName { get; set; }

        /// <summary>
        /// SQL中的参数
        /// </summary>
        public MySqlParameter[] SqlParamas { get; set; } = new MySqlParameter[] { };

        /// <summary>
        /// 当前步骤返回的DataSet结果，没有结果为null
        /// </summary>
        public DataSet DataSetResult { get; set; }

        /// <summary>
        /// 当前步骤返回的DataTable结果，没有结果为null
        /// </summary>
        public DataTable DataTableResult { get; set; }

        /// <summary>
        /// 当前步骤返回的Scalar结果，没有结果为null
        /// </summary>
        public object ScalarResult { get; set; }

        /// <summary>
        /// 当前步骤返回的SQL数据流对象结果，没有结果为null
        /// </summary>
        public MySqlDataReader DataReaderResult { get; set; }

        /// <summary>
        /// 当前步骤受影响的行数结果，没有结果为0
        /// </summary>
        public int EffectsResult { get; set; } = 0;
    }

    /// <summary>
    /// 表示事务处理的异常类型
    /// </summary>
    public class SqlTransactionError : Exception
    {
        private TransactionStep m_ErrorStep = null;

        /// <summary>
        /// 返回SqlTransactionError实例
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="errorStep">错误步骤</param>
        /// <param name="innerException">内部异常</param>
        public SqlTransactionError(string message, TransactionStep errorStep, Exception innerException = null) : base(message, innerException)
        {
            m_ErrorStep = errorStep;
        }

        /// <summary>
        /// 出错的事务步骤
        /// </summary>
        public TransactionStep ErrorStep { get { return m_ErrorStep; } }
    }

    /// <summary>
    /// 表示SQL命令返回对象类型的枚举
    /// </summary>
    public enum SqlCommandResultType : int
    {
        /// <summary>
        /// 受影响的行数/NoneQuery
        /// </summary>
        Effects = 1,
        /// <summary>
        /// 单个表
        /// </summary>
        DataTable = 2,
        /// <summary>
        /// 数据集
        /// </summary>
        DataSet = 3,
        /// <summary>
        /// 单个值
        /// </summary>
        Scalar = 4
    }
}
