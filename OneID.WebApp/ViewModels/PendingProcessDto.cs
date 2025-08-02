namespace OneID.WebApp.ViewModels
{
    public class PendingProcessDto
    {
        public string CorrelationId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string CurrentStep { get; set; } = "Admissão iniciada";

        public int ProgressPercent => CurrentStep switch
        {
            "Admissão iniciada" => 25,
            "Login provisionado" => 50,
            "Usuário provisionado no Keycloak" => 75,
            "CPF validado" => 100,
            _ => 0
        };
    }


}
