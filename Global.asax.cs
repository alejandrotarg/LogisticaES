using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using LogisticaES.Web.DataAccess;
using FluentValidation.Mvc;
using System.Web.Security;
using LogisticaES.Web.FuncionalSecurity;
using WebMatrix.WebData;
using System.Threading;
using System.Globalization;


namespace LogisticaES.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            /* Para crear la base de datos descomentar la siguiente línea y comentar la de "Disable migrations" */
            //Database.SetInitializer(new SeedData());
            Database.SetInitializer<LogisticaESContext>(null);  // Disable migrations

            using (var db = new LogisticaESContext())
            {
                db.Provincias.Count();
            }

            #region Inicializar roles y usuario admin

            // Esta verificación de roles se hace aquí por si en algún momento se agrega algún rol.
            try
            {   // En caso de que no haya sido inicializado antes en algún otro lado (necesario para actualizar WebMatrix.WebData.WebSecurity).
                WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection("LogisticaESContext", "UserProfiles", "UserId", "UserName", autoCreateTables: true);
            }
            catch { }

            foreach (FuncionalSecurity.Security.FunctionRoles _fRole in Enum.GetValues(typeof(LogisticaES.Web.FuncionalSecurity.Security.FunctionRoles)))
            {
                if (!Roles.RoleExists(_fRole.ToString()))
                    Roles.CreateRole(_fRole.ToString());
            }

            if (!WebSecurity.UserExists("adminfactory"))
            {
                WebSecurity.CreateUserAndAccount("adminfactory", "adminfactory",
                    new { Nombre = "Administrador", Apellido = "Factory" }
                    );

                Roles.AddUserToRoles("adminfactory", new string[] { "AdmUsuarios", "Factory" });
            }

            #endregion


            FluentValidationModelValidatorProvider.Configure();

        }

        protected void Application_Beginrequest(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("es-AR");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("es-AR");
        }

    }

    

    /// <summary>
    /// Contiene datos generales que se utilizan durante la sesión del usuario.
    /// Cada atributo corresponde a una variable de sesión.
    /// </summary>
    public static class SessionData
    {
        /// <summary>
        /// Indica si se está en el proceso de una nueva donación.
        /// </summary>
        public static bool IsDonationInProcess { get { return Convert.ToBoolean(HttpContext.Current.Session["DONATIONINPROC"]); } set { HttpContext.Current.Session["DONATIONINPROC"] = value; } }
   
    }
}