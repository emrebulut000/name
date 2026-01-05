namespace Uný_Proje.Web.Client.Services
{
    public class ProfilServis
    {
        public event Action? ProfilDegisti;

        public void ProfilGuncellendi()
        {
            ProfilDegisti?.Invoke();
        }
    }
}
