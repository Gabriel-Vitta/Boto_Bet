namespace Boto_da_Fortuna;

public partial class TelaInicial : ContentPage
{
    public TelaInicial()
    {
        InitializeComponent();
    }

    private async void OnComecarClicked(object sender, EventArgs e)
    {
        // Navega para a página do jogo (MainPage)
        await Navigation.PushAsync(new MainPage());
    }
}