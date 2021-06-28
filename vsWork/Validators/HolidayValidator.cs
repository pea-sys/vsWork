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
        private readonly HolidayRepository _holidayRepository;
        /// <summary>
        /// 休日状態管理
        /// </summary>
        private readonly IState<HolidaySettingState> _holidaySettingState;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HolidayValidator(IRepository<Holiday, (int, DateTime)> holidayRepository, IState<HolidaySettingState> holidaySettingState)
        {
            _holidayRepository = (HolidayRepository)holidayRepository;
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
            List<Holiday> validHoliday = (List<Holiday>)_holidayRepository.FindAll(organizationId);
            // 同日チェック
            if (validHoliday.Where(p => p.Date == dt.Date).Count() > 0)
            {
                return true;
            }
            //日付固定チェック
            var validDaySolid = validHoliday.Where(p => p.HolidayType == HolidayType.FixedDay);
            foreach (var hDay in validDaySolid)
            {
                if ((dt.Month == hDay.Date.Month) && (dt.Day == hDay.Date.Day))
                {
                    return true;
                }
            }
            // 曜日固定チェック
            // validDaySolid = (List<Holiday>)validHoliday.Where(p => p.HolidayType == HolidayType.FixedWeekofDay);
            
            return false;
        }
    }
}
