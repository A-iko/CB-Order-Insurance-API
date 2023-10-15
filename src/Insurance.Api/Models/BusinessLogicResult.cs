using Insurance.Api.BusinessLogic;

namespace Insurance.Api.Models
{
    public class BusinessLogicResult<T> where T : class
    {
        public T? data { get; set; }
        public BusinessLogicResultEnum businessLogicResult { get; set; }

        public BusinessLogicResult(BusinessLogicResultEnum businessLogicResult, T data)
        {
            this.data = data;
            this.businessLogicResult = businessLogicResult;
        }
        public BusinessLogicResult(BusinessLogicResultEnum businessLogicResult)
        {
            this.businessLogicResult = businessLogicResult;
        }
    }
}
