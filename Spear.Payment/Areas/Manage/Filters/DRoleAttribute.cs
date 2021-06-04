using Spear.Core.Exceptions;
using Spear.Core.Extensions;
using Spear.Gateway.Payment.Areas.Manage.Models;
using Spear.Payment.Contracts.Enums;
using Spear.WebApi;
using Spear.WebApi.Filters;

namespace Spear.Gateway.Payment.Areas.Manage.Filters
{
    public class DRoleAttribute : DAuthorizeAttribute
    {
        private readonly AccountRole _role;

        public DRoleAttribute(AccountRole role)
        {
            _role = role;
        }
        protected override void BaseValidate(IClientTicket ticket)
        {
            if (ticket is ManageTicket manageTicket && (manageTicket.Role.CastTo(0) & (byte)_role) == 0)
            {
                throw ErrorCodes.ClientError.CodeException();
            }
            base.BaseValidate(ticket);
        }
    }
}
