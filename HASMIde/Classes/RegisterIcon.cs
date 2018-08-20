using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HASM.Classes
{
    public class RegisterIcon
    {
        public static void RegisterHASMIcon()
        {
            try
            {
				if (PlatformSpecific.IsUNIX) return;

                if (Registry.ClassesRoot.GetSubKeyNames().Contains(".hasm"))
                    return;

                var key = Registry.ClassesRoot.CreateSubKey(".hasm");
                var icon = key.CreateSubKey("defaultIcon");

                icon.SetValue("", new FileInfo("Icons\\hasm.ico").FullName, RegistryValueKind.String);

                icon.Close();
                key.Close();
            } catch
            {
                MessageBox.Show("Cant set without admin rights");
            }
        }

    }
}
