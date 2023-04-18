using System.Text.Json;

namespace conversor_imagem
{    
    public static class Config
    {
        private static JsonDocument _config => JsonDocument.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "config.json")));

        public static string PathPastaImagens => GetConfigValue("caminhoPastaImagens");
        public static string ArquivoLogExcecao => GetConfigValue("arquivoLogExcecao");

        public static string GetConfigValue(string configKey)
        {
            return _config.RootElement.GetProperty(configKey).GetString();
        }
    }
}
