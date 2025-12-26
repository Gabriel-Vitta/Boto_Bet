namespace Boto_da_Fortuna;

public partial class MainPage : ContentPage
{
    // Variáveis globais
    double saldo = 1000.00;
    double custoAposta = 10.00;

    public MainPage()
    {
        InitializeComponent();
    }

    // Lógica do Botão GIRAR
    private async void OnApostarClicked(object sender, EventArgs e)
    {
        // 1. Segurança de saldo
        if (saldo < custoAposta)
        {
            await DisplayAlert("Fim da Linha", "Você quebrou a banca!", "Ok");
            return;
        }

        // 2. Cobra a aposta
        saldo -= custoAposta;
        LblSaldo.Text = $"Saldo: R$ {saldo:F2}";
        LblAlerta.Text = ""; // Limpa alertas anteriores

        // --- ANIMAÇÃO DA ROLETA ---

        Random rng = new Random();

        // Calcula entre 3 e 6 voltas completas + um ângulo aleatório
        double voltasCompletas = 360 * rng.Next(3, 6);
        double anguloAleatorio = rng.Next(0, 360);
        double giroTotal = voltasCompletas + anguloAleatorio;

        // Gira a IMGROLETA por 3 segundos (Física realista)
        uint duracao = 3000;
        // AQUI ESTÁ A MUDANÇA: Usamos ImgRoleta agora
        await ImgRoleta.RotateTo(giroTotal, duracao, Easing.CubicOut);

        // Reseta o ângulo matemático
        ImgRoleta.Rotation = ImgRoleta.Rotation % 360;

        // ---------------------------

        // 3. Lógica do Resultado (RTP Viciado)
        int resultado = rng.Next(1, 100);

        if (resultado > 90) // 10% de chance de ganhar (Difícil)
        {
            double premio = 15.00;
            saldo += premio;
            await DisplayAlert("GANHOU!", $"A roleta parou no prêmio! +R$ {premio:F2}", "Continuar");
        }
        else
        {
            // Perdeu (90% das vezes)
            LblAlerta.Text = "A roleta parou na área de perda. Tente novamente.";
        }

        // Atualiza o saldo final na tela
        LblSaldo.Text = $"Saldo: R$ {saldo:F2}";
    }

    // Lógica do Botão PARAR
    private async void OnPararClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert("Parar",
            "Deseja voltar ao menu inicial? (Seu saldo será resetado)",
            "Sim", "Não");

        if (confirmar)
        {
            await DisplayAlert("Resumo Educativo",
                $"Você terminou a sessão com R$ {saldo:F2}.\n\nSe isso fosse dinheiro real, você teria {(saldo < 1000 ? "PERDIDO dinheiro" : "lucrado momentaneamente")}.",
                "Voltar ao Início");

            await Navigation.PopAsync();
        }
    }
}