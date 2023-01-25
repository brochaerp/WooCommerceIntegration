using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Remoting;
using System.Configuration;
using System.Reflection;
using TkoModel;
using TkoController;
using TkoUtility;
using System.IO;
using System.Diagnostics;

namespace WooComIntegration
{
    public class AcclamareLoad
    {
        private readonly string AppStartPath;

        public AcclamareLoad()
        {
            string startPath = AppDomain.CurrentDomain.BaseDirectory;

            if (startPath.EndsWith("\\"))
                AppStartPath = startPath.Substring(0, startPath.Length - 1);
            else
                AppStartPath = startPath;

            //AppStartPath = @"C:\Acclamare Projects\Acclamare Version 2016.1.1.1\Acclamare Source Version 2016.1.1.1\Acclamare Projects\Acclamare\AcclamareClient\bin\Release";
        }

        private void ConfigureApplication(string database)
        {
            LoadConfiguration(database);
            LoadLocalInterfaces();
            LoadRemoteInterfaces();
            TkoContainer.DefaultBasePath = "C:\\Acclamare";
        }

        private void LoadConfiguration(string database)
        {
            string configFile = String.Format("{0}\\{1}.exe.config", AppStartPath, database);
            //string configFile = String.Format("{0}\\{1}.exe.config", "C:\\Acclamare", database);

            if (File.Exists(configFile))
                RemotingConfiguration.Configure(configFile, false);
            else
                throw new FileNotFoundException("Cannot find the remote configuration file!", configFile);
        }

        private void LoadLocalInterfaces()
        {
            //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //AppSettingsSection containerSettingSection = (AppSettingsSection)config.GetSection("container");

            ContainerSection section = (ContainerSection)ConfigurationManager.GetSection("container");

            if (section != null)
            {
                foreach (ContainerElement element in section.Interfaces)
                {
                    try
                    {
                        Assembly interfaceAssembly = Assembly.LoadFrom(String.Format("{0}\\{1}", AppStartPath, element.InterfaceAssembly));
                        Assembly classAssembly = Assembly.LoadFrom(String.Format("{0}\\{1}", AppStartPath, element.ConcreteClassAssembly));

                        Type interfaceType = interfaceAssembly.GetType(element.InterfaceName);
                        Type classType = classAssembly.GetType(element.ConcreteClass);

                        if (classType != null)
                            TkoContainer.Register(interfaceType, classType);
                        else
                            Debug.WriteLine(string.Format("Couldn't register interfaceType {0}!", interfaceType.ToString()), "Error");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error loading configuration!", ex);
                    }
                }
            }
        }

        private void LoadRemoteInterfaces()
        {
            foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
            {
                if (entry.ObjectType == null)
                    Debug.WriteLine(entry.ToString());
                //throw new Exception(string.Format("A configured type could not be found (for {0} [{1}]). \nPlease check spelling in the remote configuration file.", entry.TypeName, entry.ObjectUrl));
                else
                {
                    TkoContainer.Register(entry.ObjectType, entry.ObjectUrl);
                }
            }
        }

        public bool StartAcclamare(string database, string connectionString, string user, string password)
        {
            if (LoggedOnUser.MyUser == null)
            {
                ConfigureApplication(database);

                TkoContainer.RegisterConnectionString(database, connectionString);

                LoggedOnUser.LogOn(user, password);

            }

            return (LoggedOnUser.MyUser != null);
        }


    }
}
