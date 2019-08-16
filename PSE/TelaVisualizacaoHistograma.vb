Public Class TelaVisualizacaoHistograma

    'Chamada para inicialização das variáveis do Form
    Private Sub TelaVisualizacaoHistograma_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Inicializa os histogramas de R, G e B
        histogramaImagemR.Titles.Add("Histograma intensidades de R")
        histogramaImagemR.Series(0).IsVisibleInLegend = False
        histogramaImagemR.Series(0).Color = Color.Red

        histogramaImagemG.Titles.Add("Histograma intensidades de G")
        histogramaImagemG.Series(0).IsVisibleInLegend = False
        histogramaImagemG.Series(0).Color = Color.Green

        histogramaImagemB.Titles.Add("Histograma intensidades de B")
        histogramaImagemB.Series(0).IsVisibleInLegend = False
        histogramaImagemB.Series(0).Color = Color.Blue
    End Sub

    'Construtor do Form
    Public Sub New(ByVal dados As Histograma, idImagem As String)

        ' Esta chamada é requerida pelo designer.
        InitializeComponent()

        Dim intensidades(256) As Integer

        Me.Text = Me.Text + " - " + idImagem
        'Gera o histograma da imagem para a as três intensidades
        histogramaImagemR.Series(0).Points.DataBindXY(intensidades, dados.quantidadePorIntensidadeR)
        histogramaImagemG.Series(0).Points.DataBindXY(intensidades, dados.quantidadePorIntensidadeG)
        histogramaImagemB.Series(0).Points.DataBindXY(intensidades, dados.quantidadePorIntensidadeB)

    End Sub
End Class