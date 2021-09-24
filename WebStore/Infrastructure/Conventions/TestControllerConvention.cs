using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebStore.Infrastructure.Conventions
{
    public class TestControllerConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            //controller.Actions.Clear();
            //controller.Actions.Add(new ActionModel());
        }
    }
}
