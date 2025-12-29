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

    string[] simbolos = { "item1.png", "item2.png", "item3.png"};
    // Lógica do Botão GIRAR
    private async void OnApostarClicked(object sender, EventArgs e)
    {
        // 1. Verifica Saldo
        if (saldo < custoAposta)
        {
            await DisplayAlert("Ops", "Sem saldo!", "Ok");
            return;
        }

        saldo -= custoAposta;
        LblSaldo.Text = $"Saldo: R$ {saldo:F2}";
        LblAlerta.Text = "";

        // --- ANIMAÇÃO DOS SLOTS ---
        Random rng = new Random();
        Image[] slots = { Slot1, Slot2, Slot3, Slot4, Slot5, Slot6, Slot7, Slot8, Slot9 };

        // Configuração da velocidade
        int totalGiros = 30; // Quantas vezes as imagens vão trocar no total
        int delayAtual = 30; // Começa super rápido (30ms entre trocas)
        int incrementoDelay = 15; // Quanto tempo adiciona a cada giro pra ficar mais lento

        // Loop visual com desaceleração
        for (int i = 0; i < totalGiros; i++)
        {
            // 1. Troca todas as imagens
            foreach (var slot in slots)
            {
                // Adicionamos uma micro animação de escala para dar "impacto" na troca
                // Não precisa do 'await' aqui para todos fazerem ao mesmo tempo
                slot.ScaleTo(0.8, 50).ContinueWith(t => slot.ScaleTo(1.0, 50));

                int indexAleatorio = rng.Next(simbolos.Length);
                slot.Source = simbolos[indexAleatorio];
            }

            // 2. Espera o tempo atual
            await Task.Delay(delayAtual);

            // 3. Aumenta o tempo de espera para a próxima rodada (fica mais lento)
            // Só começa a desacelerar depois da metade dos giros, para garantir um começo rápido
            if (i > totalGiros / 2)
            {
                delayAtual += incrementoDelay;
                // Opcional: Se quiser que o último giro seja BEM dramático:
                if (i == totalGiros - 2) delayAtual += 300; // O penúltimo giro dá uma travada
            }
        }

        // --- VERIFICAÇÃO DE VITÓRIA ---
        // Agora vamos ver o que ficou parado na tela.
        // Precisamos saber QUAL imagem está em cada slot.
        // O jeito mais simples é verificar a propriedade Source.

        // Vamos guardar o resultado final numa matriz simples (3 linhas)
        // Nota: No MAUI, ler o Source de volta pode ser chato, então vamos confiar
        // na sorte do último giro do loop acima.

        // Lógica simplificada: Verifica Linha do Meio (Slots 4, 5 e 6) - A mais clássica
        bool ganhouLinhaMeio = VerificarLinha(Slot4, Slot5, Slot6);
        bool ganhouLinhaCima = VerificarLinha(Slot1, Slot2, Slot3);
        bool ganhouLinhaBaixo = VerificarLinha(Slot7, Slot8, Slot9);

        double premioTotal = 0;

        if (ganhouLinhaMeio) premioTotal += 100;
        if (ganhouLinhaCima) premioTotal += 50;
        if (ganhouLinhaBaixo) premioTotal += 50;

        if (premioTotal > 0)
        {
            saldo += premioTotal;
            LblAlerta.Text = $"PARABÉNS! Ganhou R$ {premioTotal:F2}";
            await DisplayAlert("JACKPOT!", $"Você ganhou R$ {premioTotal:F2}", "Uhuu");
        }
        else
        {
            LblAlerta.Text = "Não foi dessa vez. Tente de novo!";
        }

        LblSaldo.Text = $"Saldo: R$ {saldo:F2}";
        // await Banco.SalvarSaldo(saldo); // Se já tiver configurado o banco
    }

    // Função auxiliar para comparar 3 imagens
    private bool VerificarLinha(Image s1, Image s2, Image s3)
    {
        // Converte o Source para string para poder comparar
        string img1 = s1.Source.ToString();
        string img2 = s2.Source.ToString();
        string img3 = s3.Source.ToString();

        // Remove o "File: " que o MAUI coloca na frente do nome as vezes
        img1 = img1.Replace("File: ", "");
        img2 = img2.Replace("File: ", "");
        img3 = img3.Replace("File: ", "");

        return (img1 == img2 && img2 == img3);
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