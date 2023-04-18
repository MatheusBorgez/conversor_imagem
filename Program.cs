using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;

namespace conversor_imagem
{
    public class Program
    {
        static readonly List<string> tiposDeImagem = new()
        {
            ".bmp",
            ".gif",
            ".jpg",
            ".jpeg",
            ".png"
        };

        static void Main(string[] args)
        {
            try
            {
                var pathImagensRaiz = Config.PathPastaImagens;
                var diretorios = new List<string>(Directory.GetDirectories(pathImagensRaiz, "*", SearchOption.AllDirectories))
                {
                    pathImagensRaiz
                };

                foreach (var diretorio in diretorios)
                {
                    var pathConvertidas = $"{diretorio}\\Converted";

                    Console.WriteLine($"Iniciando conversão do diretório {diretorio}");

                    ConverterImagensParaWebp(diretorio, pathConvertidas);

                    Console.WriteLine($"Diretório {diretorio} covnertido com sucesso");
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
            }
        }

        private static void ConverterImagensParaWebp(string pathImagens, string pathImagensConvertidas)
        {
            CrieDiretorioImagensConvertidas(pathImagensConvertidas);
            ConverterImagens(pathImagens, pathImagensConvertidas);
            ReplaceImagens(pathImagens, pathImagensConvertidas);
            RemoverImagensAntigas(pathImagens, pathImagensConvertidas);
        }

        private static void RemoverImagensAntigas(string pathImagens, string pathImagensConvertidas)
        {
            var imagensAntigas = Directory.GetFiles(pathImagens).Where(c => tiposDeImagem.Contains(Path.GetExtension(c).ToLowerInvariant()));
            foreach (var imagemAntiga in imagensAntigas)
            {
                Console.WriteLine($"Iniciando deleção da imagem {imagemAntiga}");
                File.Delete(imagemAntiga);
                Console.WriteLine($"imagem {imagemAntiga} deletada com sucesso");
            }

            Directory.Delete(pathImagensConvertidas);
        }

        private static void ReplaceImagens(string pathImagens, string pathImagensConvertidas)
        {
            var imagensConvertidas = Directory.GetFiles(pathImagensConvertidas);
            var imagensAntigas = Directory.GetFiles(pathImagens).Where(c => tiposDeImagem.Contains(Path.GetExtension(c).ToLowerInvariant()));

            foreach (var imagemAntiga in imagensAntigas)
            {
                var nomeArquivo = Path.GetFileNameWithoutExtension(imagemAntiga);
                var imagemConvertida = imagensConvertidas
                    .FirstOrDefault(c => Path.GetFileNameWithoutExtension(c) == nomeArquivo);


                if (imagemConvertida != null)
                {
                    Console.WriteLine($"Iniciando substituição de formato da imagem {imagemAntiga}");
                    File.Move(imagemConvertida, Path.Combine(Path.GetDirectoryName(imagemAntiga), Path.GetFileName(imagemConvertida)), true);
                    Console.WriteLine($"Imagem substituida com sucesso!");
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

        private static void CrieDiretorioImagensConvertidas(string pathImagensConvertidas)
        {
            if (!Directory.Exists(pathImagensConvertidas))
            {
                Directory.CreateDirectory(pathImagensConvertidas);
            }
        }

        private static void ConverterImagens(string pathImagens, string pathImagensConvertidas)
        {
            var imagensAntigas = Directory.GetFiles(pathImagens).Where(c => tiposDeImagem.Contains(Path.GetExtension(c).ToLowerInvariant()));

            foreach (var imagemParaConverter in imagensAntigas)
            {
                Console.WriteLine($"Iniciando conversão do arquivo {imagemParaConverter}");

                using var webPFileStream = new FileStream($"{pathImagens}\\{Path.GetFileNameWithoutExtension(imagemParaConverter)}.webp", FileMode.Create);
                using var imageFactory = new ImageFactory(preserveExifData: false);
                using var file = File.OpenRead(imagemParaConverter);

                imageFactory.Load(file)
                            .Format(new WebPFormat())
                            .Quality(80)
                            .Save(webPFileStream);

                Console.WriteLine($"Arquivo {imagemParaConverter} convertido com sucesso!");
            }
        }

    }
}