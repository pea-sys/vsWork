using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using vsWork.Data;
using vsWork.Utils;
namespace vsWork.Features.Organizations.Validator
{
    public class OrganizationValidator : AbstractValidator<Organization>
    {
        /// <summary>
        /// ユーザリポジトリ
        /// </summary>
        private readonly IRepository<Organization, string> _organizationRepository;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userRepository">ユーザリポジトリ</param>
        public OrganizationValidator(IRepository<Organization, string> organizationRepository)
        {
            _organizationRepository = organizationRepository;

            RuleFor(x => x.OrganizationId).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("組織IDを入力してください。")
                 .DependentRules(() =>
                 {
                     RuleFor(x => new { x.OrganizationId, x.DataOperation }).Must(x => IsExistId(x.OrganizationId, x.DataOperation)).WithMessage("登録済みのIDです");
                 });
            RuleFor(x => x.OrganizationName).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("組織名称を入力してください。");

        }
        /// <summary>
        /// ユーザの存在チェック
        /// </summary>
        /// <param name="id"></param>
        /// <returns>false:正常 true:異常</returns>
        private bool IsExistId(string id, EnumDataOperation eOperation)
        {
            if (eOperation == EnumDataOperation.Create)
            {
                Organization validOrganization = _organizationRepository.FindById(id);
                return (validOrganization == null);
            }
            else
            {
                return true;
            }
        }
    }
}
