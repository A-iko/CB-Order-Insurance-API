using Insurance.Api.BusinessLogic;

namespace Insurance.Api.Models
{
    public class InsuranceCalculatorResult<T> where T : class
    {
        public T? data { get; set; }
        public InsuranceCalculatorResultEnum insuranceCalculatorResult { get; set; }

        public InsuranceCalculatorResult(InsuranceCalculatorResultEnum insuranceCalculatorResult, T data)
        {
            this.data = data;
            this.insuranceCalculatorResult = insuranceCalculatorResult;
        }
        public InsuranceCalculatorResult(InsuranceCalculatorResultEnum insuranceCalculatorResult)
        {
            this.insuranceCalculatorResult = insuranceCalculatorResult;
        }
    }
}
