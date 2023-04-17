using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using System.Text.Json;

namespace conversor_imagem
{
    public class Program
    {
        static void Main(string[] args)
        {
            var configPath = Path.Combine(Environment.CurrentDirectory, "config.json");
            var config = JsonDocument.Parse(File.ReadAllText(configPath));

            var pathPastaImagens = config.RootElement.GetProperty("caminhoPastaImagens").GetString();
            var novoPathPastaImagens = config.RootElement.GetProperty("novoCaminhoPastaImagens").GetString();
            var arquivoLog = config.RootElement.GetProperty("arquivoLogExcecao").GetString() ?? Environment.CurrentDirectory;

            try
            {
                CrieDiretorioNovo(novoPathPastaImagens);
                ConverterImagens(pathPastaImagens, novoPathPastaImagens);
            }
            catch (Exception ex)
            {
                WriteExceptionLog(arquivoLog, ex);
            }
        }

        private static void WriteExceptionLog(string? arquivoLog, Exception ex)
        {
            var logExcecao = new StreamWriter(arquivoLog);

            logExcecao.WriteLine(@$"{ex.Message}
                                        {ex.InnerException}
                                        {ex.StackTrace}");

            logExcecao.Close();
        }

        private static void CrieDiretorioNovo(string novoPathPastaImagens)
        {
            if (!Directory.Exists(novoPathPastaImagens))
            {
                Directory.CreateDirectory(novoPathPastaImagens);
            }
        }

        private static void ConverterImagens(string pathPastaImagens, string novoPathPastaImagens)
        {
            var pathImagens = Directory.GetFiles(pathPastaImagens);

            foreach (var pathImagem in pathImagens.Where(c => !c.Contains("webp")))
            {
                using var webPFileStream = new FileStream($"{novoPathPastaImagens}\\{Path.GetFileNameWithoutExtension(pathImagem)}.webp", FileMode.Create);
                using var imageFactory = new ImageFactory(preserveExifData: false);

                imageFactory.Load(File.OpenRead(pathImagem))
                            .Format(new WebPFormat())
                            .Quality(80)
                            .Save(webPFileStream);
            }
        }
    }
}