# Bravetech.Report.PdfGenerator

Pacote NuGet para geração de relatórios PDF a partir de HTML usando iText7, com suporte a:

- Cabeçalho e rodapé personalizados
- Orientação retrato ou paisagem
- Margens padrão

## Instalação

```
dotnet add package Bravetech.Report.PdfGenerator
```

## Uso

```csharp
var options = new RelatorioOptions
{
    HeaderText = "Relatório Bravetech",
    FooterText = "Confidencial",
    Portrait = true
};

var service = new PdfGeneratorService(Options.Create(options));
byte[] pdf = service.GerarPdf(html);
File.WriteAllBytes("relatorio.pdf", pdf);
```

## Licença

Este projeto é licenciado sob a AGPLv3.
