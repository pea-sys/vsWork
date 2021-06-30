using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Fluxor;
using vsWork.Data;
using vsWork.Stores;
using vsWork.Utils;
namespace vsWork.Validators
{
    public class OrganizationValidator : AbstractValidator<Organization>
    {
        /// <summary>
        /// 組織リポジトリ
        /// </summary>
        private readonly IRepository<Organization, int> _organizationRepository;
        /// <summary>
        /// 組織状態管理
        /// </summary>
        private readonly IState<OrganizationSettingState> _organizationSettingState;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="organizationRepository">ユーザリポジトリ</param>
        public OrganizationValidator(IRepository<Organization, int> organizationRepository, IState<OrganizationSettingState> organizationSettingState)
        {
            _organizationRepository = organizationRepository;
            _organizationSettingState = organizationSettingState;

            if (_organizationSettingState.Value.Mode == SettingMode.Add)
            {
                RuleFor(x => x.OrganizationId).Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("組織IDを入力してください。")
                   .DependentRules(() =>
                   {
                       RuleFor(x => x.OrganizationId).Must(x => IsExistId(x)).WithMessage("登録済みのIDです");
                   });

                RuleFor(x => x.OrganizationName).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("組織名称を入力してください。");
            }
            else if (organizationSettingState.Value.Mode == SettingMode.Update)
            {

                RuleFor(x => x.OrganizationName).Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("組織名称を入力してください。");
            }
        }
        /// <summary>
        /// 組織の存在チェック
        /// </summary>
        /// <param name="id"></param>
        /// <returns>false:正常 true:異常</returns>
        private bool IsExistId(int id)
        {
            Organization validOrganization = _organizationRepository.FindById(id);
            return (validOrganization == null);
        }
    }
}
