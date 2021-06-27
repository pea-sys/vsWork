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
    public class HolidayValidator : AbstractValidator<Holiday>
    {
        /// <summary>
        /// 休日リポジトリ
        /// </summary>
        private readonly IRepository<Holiday, (int,DateTime)> _holidayRepository;
        /// <summary>
        /// 休日状態管理
        /// </summary>
        private readonly IState<HolidaySettingState> _holidaySettingState;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HolidayValidator(IRepository<Holiday, (int, DateTime)> holidayRepository, IState<HolidaySettingState> holidaySettingState)
        {
            _holidayRepository = holidayRepository;
            _holidaySettingState = holidaySettingState;

            if (_holidaySettingState.Value.Mode == SettingMode.Add)
            {
                RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("休日名称を入力してください。")
                   .DependentRules(() =>
                   {
                       RuleFor(x => new { x.OrganizationId, x.Date }).Must(x => IsExistId(x.OrganizationId, x.Date)).WithMessage("登録済みの日付です");
                   });
            }
            else
            {
                RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("休日名称を入力してください。");
            }
        }
        /// <summary>
        /// 組織の存在チェック
        /// </summary>
        /// <param name="id"></param>
        /// <returns>false:正常 true:異常</returns>
        private bool IsExistId(int organizationId, DateTime dt)
        {
            Holiday validHoliday = _holidayRepository.FindById((organizationId,dt));
            return (validHoliday == null);
        }
    }
}
