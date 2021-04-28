using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Gateway.Payment.Areas.Manage.Models;
using Acb.Payment.Contracts.Enums;
using Acb.WebApi;
using Acb.WebApi.Filters;

namespace Acb.Gateway.Payment.Areas.Manage.Filters
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
