using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using System.Linq;

namespace conversor_imagem
{
    public class Program
    {
        static List<string> tiposDeImagem = new()
        {
            ".bmp",
            ".gif",
            ".jpg",
            ".jpeg",
            ".png"
        };

        static void Main(string[] args)
        {
            CrieDiretorioNovo();
            ConverterImagens();
            ReplaceImagens();
            RemoverImagensAntigas();
        }

        private static void RemoverImagensAntigas()
        {
            var imagensAntigas = Directory.GetFiles(Config.PathPastaImagens).Where(c => !Path.GetExtension(c).Contains("webp"));
            foreach (var imagemAntiga in imagensAntigas)
            {
                try
                {
                    File.Delete(imagemAntiga);
                }
                catch (Exception ex)
                {
                    WriteExceptionLog(ex);
                }
            }

            Directory.Delete(Config.PathPastaImagensConvertidas);
        }

        private static void ReplaceImagens()
        {
            var imagensConvertidas = Directory.GetFiles(Config.PathPastaImagensConvertidas);
            var imagensAntigas = Directory.GetFiles(Config.PathPastaImagens).Where(c => !Path.GetExtension(c).Contains("webp"));

            foreach (var imagemAntiga in imagensAntigas)
            {
                var nomeArquivo = Path.GetFileNameWithoutExtension(imagemAntiga);
                var imagemConvertida = imagensConvertidas
                    .FirstOrDefault(c => Path.GetFileNameWithoutExtension(c) == nomeArquivo);

                if (imagemConvertida != null)
                {
                    try
                    {
                        File.Move(imagemConvertida, Path.Combine(Path.GetDirectoryName(imagemAntiga), Path.GetFileName(imagemConvertida)), true);
                    }
                    catch (Exception ex)
                    {
                        WriteExceptionLog(ex);
                    }
                }
            }
        }

        private static void WriteExceptionLog(Exception ex)
        {
            var arquivoLog = Config.ArquivoLogExcecao;

            var logExcecao = new StreamWriter(arquivoLog);

            logExcecao.WriteLine(@$"{ex.Message}
                                        {ex.InnerException}
                                        {ex.StackTrace}");

            logExcecao.Close();
        }

        private static void CrieDiretorioNovo()
        {
            if (!Directory.Exists(Config.PathPastaImagensConvertidas))
            {
                Directory.CreateDirectory(Config.PathPastaImagensConvertidas);
            }
        }

        private static void ConverterImagens()
        {
            var pathImagens = Directory.GetFiles(Config.PathPastaImagens).Where(c => tiposDeImagem.Contains(Path.GetExtension(c).ToLowerInvariant()));

            foreach (var pathImagem in pathImagens)
            {
                using var webPFileStream = new FileStream($"{Config.PathPastaImagensConvertidas}\\{Path.GetFileNameWithoutExtension(pathImagem)}.webp", FileMode.Create);
                using var imageFactory = new ImageFactory(preserveExifData: false);
                using var file = File.OpenRead(pathImagem);

                imageFactory.Load(file)
                            .Format(new WebPFormat())
                            .Quality(80)
                            .Save(webPFileStream);
            }
        }
    }
}