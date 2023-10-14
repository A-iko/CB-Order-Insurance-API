using Insurance.Api.BusinessLogic;
using Insurance.Api.Models.ProductApi;

namespace Insurance.Api.Models
{
    public class InsuranceDtoResult
    {
        public InsuranceDto? insuranceDto { get; set; }
        public InsuranceCalculatorResult insuranceCalculatorResult { get; set; }

        public InsuranceDtoResult(InsuranceCalculatorResult insuranceCalculatorResult, InsuranceDto insuranceDto)
        {
            this.insuranceDto = insuranceDto;
            this.insuranceCalculatorResult = insuranceCalculatorResult;
        }
        public InsuranceDtoResult(InsuranceCalculatorResult insuranceCalculatorResult)
        {
            this.insuranceCalculatorResult = insuranceCalculatorResult;
        }
    }
}
