using System.Linq.Expressions;

namespace Lims.ToolsForClient
{
    /// <summary>
    /// 计算体积参数
    /// </summary>
    public class ExpressionVolume
    {
        #region 私有类

        /// <summary>
        /// 体积参数类
        /// </summary>
        public class VolumeClass
        {
            /// <summary>
            /// 不可以去掉
            /// Expression合成方法时需要支持无参实例化
            /// </summary>
            public VolumeClass()
            {
            }

            /// <summary>
            /// 实例化
            /// </summary>
            /// <param name="Long">长</param>
            /// <param name="Width">宽</param>
            /// <param name="Height">高</param>
            public VolumeClass(decimal? Long, decimal? Width, decimal? Height, decimal? Wt_Weight = null)
            {
                this.L = Long ?? 0;
                this.W = Width ?? 0;
                this.H = Height ?? 0;
                this.Wt_Weight = Wt_Weight ?? 0;
            }

            /// <summary>
            /// 长
            /// </summary>
            /// <remarks>
            /// 不能随意改变字段名
            /// Expression合成方法时需要
            /// </remarks>
            public decimal L
            {
                get; set;
            }

            /// <summary>
            /// 宽
            /// </summary>
            /// <remarks>
            /// 不能随意改变字段名
            /// Expression合成方法时需要
            /// </remarks>
            public decimal W
            {
                get; set;
            }

            /// <summary>
            /// 高
            /// </summary>
            /// <remarks>
            /// 不能随意改变字段名
            /// Expression合成方法时需要
            /// </remarks>
            public decimal H
            {
                get; set;
            }

            /// <summary>
            /// 重量
            /// </summary>
            public decimal Wt_Weight
            {
                get; set;
            }
        }

        #endregion 私有类

        /// <summary>
        /// 线程锁，防止缓存键值对时异常
        /// </summary>
        private static readonly object Lock = new object { };

        /// <summary>
        /// 缓存表达式(比较)
        /// </summary>
        private static readonly Dictionary<string, Func<VolumeClass, bool>> DicEqual = new Dictionary<string, Func<VolumeClass, bool>>();

        /// <summary>
        /// 比较表达式是否成立
        /// </summary>
        /// <param name="expressionString">表达式</param>
        /// <param name="volume">体积参数</param>
        /// <returns></returns>
        public bool InvokeEqual(string expressionString, VolumeClass volume)
        {
            if (DicEqual.ContainsKey(expressionString))
            {
                return DicEqual[expressionString].Invoke(volume);
            }

            var strs = expressionString.Split('>', '<');

            if (strs.Length != 2)
            {
                DicEqual.Add(expressionString, t => false);
                return false;
            }

            //定义体积参数
            ParameterExpression parameterExpression = Expression.Parameter(typeof(VolumeClass), "volume");
            Expression expression = GetExpression(strs[0], parameterExpression);

            //是否大于
            bool IsGreaterThan = expressionString.Contains('>');

            //是否允许相等
            bool IsEqual = strs[1].StartsWith("=");
            strs[1] = strs[1].TrimStart('=');

            decimal Equal = decimal.Parse(strs[1]);

            if (IsGreaterThan)
            {
                if (IsEqual)
                {
                    expression = Expression.GreaterThanOrEqual(expression, Expression.Constant(Equal, typeof(decimal)));
                }
                else
                {
                    expression = Expression.GreaterThan(expression, Expression.Constant(Equal, typeof(decimal)));
                }
            }
            else
            {
                if (IsEqual)
                {
                    expression = Expression.LessThanOrEqual(expression, Expression.Constant(Equal, typeof(decimal)));
                }
                else
                {
                    expression = Expression.LessThan(expression, Expression.Constant(Equal, typeof(decimal)));
                }
            }
            //获得Lambda表达式
            Expression<Func<VolumeClass, bool>> Lambda = Expression.Lambda<Func<VolumeClass, bool>>(expression, parameterExpression);

            //生成对应的委托
            var func = Lambda.Compile();

            //防止并发导致异常
            if (!DicEqual.ContainsKey(expressionString))
            {
                lock (Lock)
                {
                    if (!DicEqual.ContainsKey(expressionString))
                    {
                        //缓存委托
                        DicEqual.Add(expressionString, func);
                    }
                }
            }

            return func.Invoke(volume);
        }

        /// <summary>
        /// 缓存表达式(计算值)
        /// </summary>
        private static readonly Dictionary<string, Func<VolumeClass, decimal>> DicOperator = new Dictionary<string, Func<VolumeClass, decimal>>();

        /// <summary>
        /// 得到表达式左侧的值
        /// </summary>
        /// <param name="expressionString">表达式</param>
        /// <param name="volume">体积参数</param>
        /// <returns></returns>
        public decimal InvokeOperator(string expressionString, VolumeClass volume)
        {
            if (DicOperator.ContainsKey(expressionString))
            {
                return DicOperator[expressionString].Invoke(volume);
            }

            var strs = expressionString.Split('>', '<');

            if (strs.Length != 2)
            {
                DicOperator.Add(expressionString, t => -1);
                return -1;
            }

            //定义体积参数
            ParameterExpression parameterExpression = Expression.Parameter(typeof(VolumeClass), "volume");
            System.Linq.Expressions.Expression expression = GetExpression(strs[0], parameterExpression);

            //获得Lambda表达式
            Expression<Func<VolumeClass, decimal>> Lambda = Expression.Lambda<Func<VolumeClass, decimal>>(expression, parameterExpression);

            //生成对应的委托
            var func = Lambda.Compile();

            //防止并发导致异常
            if (!DicOperator.ContainsKey(expressionString))
            {
                lock (Lock)
                {
                    if (!DicOperator.ContainsKey(expressionString))
                    {
                        //缓存委托
                        DicOperator.Add(expressionString, func);
                    }
                }
            }
            return func.Invoke(volume);
        }

        /// <summary>
        /// 验证表达式
        /// </summary>
        /// <param name="expressionString">表达式</param>
        /// <returns></returns>
        public decimal Verification(string expressionString)
        {
            var strs = expressionString.Split('>', '<');

            if (strs.Length != 2)
            {
                DicOperator.Add(expressionString, t => -1);
                return -1;
            }
            if (string.IsNullOrWhiteSpace(strs[1]))
            {
                return -1;
            }

            //定义体积参数
            ParameterExpression parameterExpression = Expression.Parameter(typeof(VolumeClass), "volume");
            System.Linq.Expressions.Expression expression = GetExpression(strs[0], parameterExpression);

            //获得Lambda表达式
            Expression<Func<VolumeClass, decimal>> Lambda = Expression.Lambda<Func<VolumeClass, decimal>>(expression, parameterExpression);

            //生成对应的委托
            var func = Lambda.Compile();

            return func.Invoke(new VolumeClass(1, 1, 1));
        }

        #region 私有方法

        /// <summary>
        /// 生成公共表达式
        /// </summary>
        /// <param name="str"></param>
        /// <param name="parameterExpression">定义体积参数</param>
        /// <returns></returns>
        private System.Linq.Expressions.Expression GetExpression(string str, ParameterExpression parameterExpression)
        {
            if (str.StartsWith("+") || str.StartsWith("-") || str.StartsWith("*") || str.StartsWith("/"))
            {
                throw new Exception("表达式不能已符号开头");
            }
            Expression expression = null;
            foreach (var Add in str.Split('+'))
            {
                if (Add.Contains('-'))
                {
                    //假如是第一次进入则表示还是+
                    bool IsFirst = true;
                    foreach (var Subtract in Add.Split('-'))
                    {
                        Expression _expression = null;
                        if (Subtract.Contains('*'))
                        {
                            _expression = MultiplyAction(Subtract, parameterExpression);
                        }
                        else if (Subtract.Contains('/'))
                        {
                            _expression = DivideAction(Subtract, parameterExpression);
                        }
                        else
                        {
                            _expression = GetPropertyOrConstant(Subtract, parameterExpression);
                        }
                        if (IsFirst)
                        {
                            expression = expression == null ? _expression : Expression.Add(expression, _expression);
                        }
                        else
                        {
                            //不是第一次进入不可能是null
                            expression = Expression.Subtract(expression, _expression);
                        }
                        IsFirst = false;
                    }
                }
                else if (Add.Contains('*'))
                {
                    Expression _expression = MultiplyAction(Add, parameterExpression);
                    expression = expression == null ? _expression : Expression.Add(expression, _expression);
                }
                else if (Add.Contains('/'))
                {
                    Expression _expression = DivideAction(Add, parameterExpression);
                    expression = expression == null ? _expression : Expression.Add(expression, _expression);
                }
                else
                {
                    //拼接+
                    Expression _expression = GetPropertyOrConstant(Add, parameterExpression);
                    expression = expression == null ? _expression : Expression.Add(expression, _expression);
                }
            }

            return expression;
        }

        /// <summary>
        /// 乘
        /// </summary>
        /// <param name="str"></param>
        private System.Linq.Expressions.Expression MultiplyAction(string str, ParameterExpression parameterExpression)
        {
            List<Expression> list = new List<Expression>();
            foreach (var Multiply in str.Split('*'))
            {
                list.Add(Multiply.Contains('/') ? DivideAction(Multiply, parameterExpression) : GetPropertyOrConstant(Multiply, parameterExpression));
            }
            Expression MultiplyExpression = list[0];
            for (int i = 1; i < list.Count; i++)
            {
                MultiplyExpression = Expression.Multiply(MultiplyExpression, list[i]);
            }
            return MultiplyExpression;
        }

        /// <summary>
        /// 除
        /// </summary>
        /// <param name="str"></param>
        private System.Linq.Expressions.Expression DivideAction(string str, ParameterExpression parameterExpression)
        {
            string[] strs = str.Split('/');
            return Expression.Divide(GetPropertyOrConstant(strs[0], parameterExpression), GetPropertyOrConstant(strs[1], parameterExpression));
        }

        /// <summary>
        /// 获取属性或常量
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="parameterExpression">变量参数</param>
        /// <returns></returns>
        private System.Linq.Expressions.Expression GetPropertyOrConstant(string value, ParameterExpression parameterExpression)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception("符号不能相连");
            }
            Expression expression = null;
            if (value == "L" || value == "W" || value == "H")
            {
                //读取变量的属性
                expression = Expression.Property(parameterExpression, value);
            }
            else
            {
                try
                {
                    expression = Expression.Constant(decimal.Parse(value), typeof(decimal));
                }
                catch (Exception)
                {
                    throw new Exception("长宽高不能相连");
                }
            }
            return expression;
        }

        #endregion 私有方法
    }
}