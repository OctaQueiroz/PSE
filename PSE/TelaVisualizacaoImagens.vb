Public Class TelaVisualizacaoImagens
    Dim imagemParaSalvar As Image

    'Inicializa as configurações do form
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SaveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif"
        SaveFileDialog1.Title = "Save an Image File"
    End Sub

    'COntrutor do Form, recebe o parametro da imagem que será mostrada 
    Public Sub New(ByVal imagem As Image, idImagem As String)

        If (imagem.Height < 735 And imagem.Width < 1536) Then
            InitializeComponent()
            Me.Height = imagem.Height + 47 + 58
            Me.Width = imagem.Width + 3
            frameDemonstracao.Image = imagem
        Else
            InitializeComponent()
            'Maximiza a janela
            Me.WindowState = 2

            'Redimensiona o quadro para o tamanho da imagem
            frameDemonstracao.Height = imagem.Height
            frameDemonstracao.Width = imagem.Width

            'Como inicializa em AutoSize, devese colocar como normal para que as alterações de tamanho feitas acima tenham efeito
            frameDemonstracao.SizeMode = PictureBoxSizeMode.Normal

            frameDemonstracao.Image = imagem

        End If
        Me.Text = Me.Text + " - " + idImagem
        imagemParaSalvar = imagem
    End Sub

    'Botão para slavar a imagem mostrada na tela
    Private Sub salvar(sender As Object, e As EventArgs) Handles save.Click
        SaveFileDialog1.FileName = "alteração.jpg"
        SaveFileDialog1.ShowDialog()
        Dim altura As Integer = Me.Height
        Dim largura As Integer = Me.Width

        If SaveFileDialog1.FileName <> "" Then
            Dim fs As System.IO.FileStream = CType(SaveFileDialog1.OpenFile(), System.IO.FileStream)

            'Salva a ultima alteração realizada na imagem original no formato .jpg
            imagemParaSalvar.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg)

            fs.Close()
        End If
    End Sub
End Class