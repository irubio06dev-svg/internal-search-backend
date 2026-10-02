namespace internal_search.Domain.Configuration
{
    public class RecuperacionClaveOptions
    {
        // URL de la pantalla del frontend que recibe ?token=...
        public string FrontendResetUrl { get; set; } = "http://localhost:4200/auth/reset-password";
        public int VigenciaRecuperacionMinutos { get; set; } = 30;
        public int VigenciaInvitacionHoras { get; set; } = 72;
        public string NombreSistema { get; set; } = "Buscador Interno";
    }
}
