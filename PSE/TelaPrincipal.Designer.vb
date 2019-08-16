<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class janelaPse
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(janelaPse))
        Dim ChartArea22 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend22 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series22 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim ChartArea23 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend23 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series23 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim ChartArea24 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend24 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series24 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.media = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.maximo = New System.Windows.Forms.Button()
        Me.minimo = New System.Windows.Forms.Button()
        Me.mediana = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.grupoPassaBaixa = New System.Windows.Forms.GroupBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.dimensoesMascara = New System.Windows.Forms.ComboBox()
        Me.gaussiano = New System.Windows.Forms.Button()
        Me.grupoErosao = New System.Windows.Forms.GroupBox()
        Me.fechamento = New System.Windows.Forms.Button()
        Me.abertura = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.frameFoto2 = New System.Windows.Forms.PictureBox()
        Me.frameFoto1 = New System.Windows.Forms.PictureBox()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.tabelaHistorico = New System.Windows.Forms.DataGridView()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.histogramaImagemR = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.histogramaImagemG = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.histogramaImagemB = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.dica = New System.Windows.Forms.ToolTip(Me.components)
        Me.binarizar = New System.Windows.Forms.Button()
        Me.negativo = New System.Windows.Forms.Button()
        Me.tonsDeCinza = New System.Windows.Forms.Button()
        Me.highBoost = New System.Windows.Forms.Button()
        Me.laplace = New System.Windows.Forms.Button()
        Me.sobel = New System.Windows.Forms.Button()
        Me.reciclar = New System.Windows.Forms.Button()
        Me.desfazer = New System.Windows.Forms.Button()
        Me.novaImagem = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.grupoIntensidades = New System.Windows.Forms.GroupBox()
        Me.grupoPassaAlta = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.pesosHighBoost = New System.Windows.Forms.ComboBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.progressoImagem = New System.Windows.Forms.ProgressBar()
        Me.grupoPassaBaixa.SuspendLayout()
        Me.grupoErosao.SuspendLayout()
        CType(Me.frameFoto2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.frameFoto1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tabelaHistorico, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.histogramaImagemR, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.histogramaImagemG, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.histogramaImagemB, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grupoIntensidades.SuspendLayout()
        Me.grupoPassaAlta.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.SuspendLayout()
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'media
        '
        Me.media.Location = New System.Drawing.Point(20, 21)
        Me.media.Margin = New System.Windows.Forms.Padding(4)
        Me.media.Name = "media"
        Me.media.Size = New System.Drawing.Size(193, 54)
        Me.media.TabIndex = 6
        Me.media.Text = "Filtro de Media"
        Me.dica.SetToolTip(Me.media, resources.GetString("media.ToolTip"))
        Me.media.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(857, 98)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(113, 17)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Imagem alterada"
        '
        'maximo
        '
        Me.maximo.Location = New System.Drawing.Point(23, 23)
        Me.maximo.Margin = New System.Windows.Forms.Padding(4)
        Me.maximo.Name = "maximo"
        Me.maximo.Size = New System.Drawing.Size(193, 54)
        Me.maximo.TabIndex = 11
        Me.maximo.Text = "Filtro de Máximo"
        Me.dica.SetToolTip(Me.maximo, resources.GetString("maximo.ToolTip"))
        Me.maximo.UseVisualStyleBackColor = True
        '
        'minimo
        '
        Me.minimo.Location = New System.Drawing.Point(240, 23)
        Me.minimo.Margin = New System.Windows.Forms.Padding(4)
        Me.minimo.Name = "minimo"
        Me.minimo.Size = New System.Drawing.Size(193, 54)
        Me.minimo.TabIndex = 12
        Me.minimo.Text = "Filtro de Mínimo"
        Me.dica.SetToolTip(Me.minimo, resources.GetString("minimo.ToolTip"))
        Me.minimo.UseVisualStyleBackColor = True
        '
        'mediana
        '
        Me.mediana.Location = New System.Drawing.Point(239, 21)
        Me.mediana.Margin = New System.Windows.Forms.Padding(4)
        Me.mediana.Name = "mediana"
        Me.mediana.Size = New System.Drawing.Size(193, 54)
        Me.mediana.TabIndex = 13
        Me.mediana.Text = "Filtro de Mediana"
        Me.dica.SetToolTip(Me.mediana, resources.GetString("mediana.ToolTip"))
        Me.mediana.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(238, 101)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(178, 17)
        Me.Label3.TabIndex = 29
        Me.Label3.Text = "Peso para filtro High-Boost"
        '
        'grupoPassaBaixa
        '
        Me.grupoPassaBaixa.Controls.Add(Me.Label7)
        Me.grupoPassaBaixa.Controls.Add(Me.dimensoesMascara)
        Me.grupoPassaBaixa.Controls.Add(Me.gaussiano)
        Me.grupoPassaBaixa.Controls.Add(Me.mediana)
        Me.grupoPassaBaixa.Controls.Add(Me.media)
        Me.grupoPassaBaixa.Location = New System.Drawing.Point(495, 810)
        Me.grupoPassaBaixa.Margin = New System.Windows.Forms.Padding(4)
        Me.grupoPassaBaixa.Name = "grupoPassaBaixa"
        Me.grupoPassaBaixa.Padding = New System.Windows.Forms.Padding(4)
        Me.grupoPassaBaixa.Size = New System.Drawing.Size(452, 161)
        Me.grupoPassaBaixa.TabIndex = 20
        Me.grupoPassaBaixa.TabStop = False
        Me.grupoPassaBaixa.Text = "Filtros Passa-Baixa"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(237, 101)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(156, 17)
        Me.Label7.TabIndex = 18
        Me.Label7.Text = "Dimensões da máscara"
        '
        'dimensoesMascara
        '
        Me.dimensoesMascara.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.dimensoesMascara.FormattingEnabled = True
        Me.dimensoesMascara.Items.AddRange(New Object() {"3x3", "5x5", "7x7"})
        Me.dimensoesMascara.Location = New System.Drawing.Point(240, 121)
        Me.dimensoesMascara.Name = "dimensoesMascara"
        Me.dimensoesMascara.Size = New System.Drawing.Size(193, 24)
        Me.dimensoesMascara.TabIndex = 17
        '
        'gaussiano
        '
        Me.gaussiano.Location = New System.Drawing.Point(20, 91)
        Me.gaussiano.Margin = New System.Windows.Forms.Padding(4)
        Me.gaussiano.Name = "gaussiano"
        Me.gaussiano.Size = New System.Drawing.Size(193, 54)
        Me.gaussiano.TabIndex = 14
        Me.gaussiano.Text = "Filtro Gaussiano"
        Me.dica.SetToolTip(Me.gaussiano, resources.GetString("gaussiano.ToolTip"))
        Me.gaussiano.UseVisualStyleBackColor = True
        '
        'grupoErosao
        '
        Me.grupoErosao.Controls.Add(Me.fechamento)
        Me.grupoErosao.Controls.Add(Me.abertura)
        Me.grupoErosao.Controls.Add(Me.maximo)
        Me.grupoErosao.Controls.Add(Me.minimo)
        Me.grupoErosao.Location = New System.Drawing.Point(495, 641)
        Me.grupoErosao.Margin = New System.Windows.Forms.Padding(4)
        Me.grupoErosao.Name = "grupoErosao"
        Me.grupoErosao.Padding = New System.Windows.Forms.Padding(4)
        Me.grupoErosao.Size = New System.Drawing.Size(452, 161)
        Me.grupoErosao.TabIndex = 21
        Me.grupoErosao.TabStop = False
        Me.grupoErosao.Text = "Filtros de Dilatação / Erosão"
        '
        'fechamento
        '
        Me.fechamento.Location = New System.Drawing.Point(239, 91)
        Me.fechamento.Margin = New System.Windows.Forms.Padding(4)
        Me.fechamento.Name = "fechamento"
        Me.fechamento.Size = New System.Drawing.Size(193, 54)
        Me.fechamento.TabIndex = 14
        Me.fechamento.Text = "Filtro de Fechamento"
        Me.dica.SetToolTip(Me.fechamento, resources.GetString("fechamento.ToolTip"))
        Me.fechamento.UseVisualStyleBackColor = True
        '
        'abertura
        '
        Me.abertura.Location = New System.Drawing.Point(23, 91)
        Me.abertura.Margin = New System.Windows.Forms.Padding(4)
        Me.abertura.Name = "abertura"
        Me.abertura.Size = New System.Drawing.Size(193, 54)
        Me.abertura.TabIndex = 13
        Me.abertura.Text = "Filtro de Abertura"
        Me.dica.SetToolTip(Me.abertura, resources.GetString("abertura.ToolTip"))
        Me.abertura.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(-563, 15)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(107, 17)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Imagem original"
        '
        'frameFoto2
        '
        Me.frameFoto2.BackColor = System.Drawing.SystemColors.ActiveBorder
        Me.frameFoto2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.frameFoto2.Location = New System.Drawing.Point(860, 119)
        Me.frameFoto2.Margin = New System.Windows.Forms.Padding(4)
        Me.frameFoto2.Name = "frameFoto2"
        Me.frameFoto2.Size = New System.Drawing.Size(799, 491)
        Me.frameFoto2.TabIndex = 7
        Me.frameFoto2.TabStop = False
        '
        'frameFoto1
        '
        Me.frameFoto1.BackColor = System.Drawing.SystemColors.ActiveBorder
        Me.frameFoto1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.frameFoto1.Location = New System.Drawing.Point(26, 119)
        Me.frameFoto1.Margin = New System.Windows.Forms.Padding(4)
        Me.frameFoto1.Name = "frameFoto1"
        Me.frameFoto1.Size = New System.Drawing.Size(799, 491)
        Me.frameFoto1.TabIndex = 0
        Me.frameFoto1.TabStop = False
        '
        'ImageList1
        '
        Me.ImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        Me.ImageList1.ImageSize = New System.Drawing.Size(16, 16)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        '
        'tabelaHistorico
        '
        Me.tabelaHistorico.AllowUserToResizeColumns = False
        Me.tabelaHistorico.AllowUserToResizeRows = False
        Me.tabelaHistorico.BackgroundColor = System.Drawing.Color.LightGray
        Me.tabelaHistorico.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.tabelaHistorico.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.tabelaHistorico.Location = New System.Drawing.Point(995, 662)
        Me.tabelaHistorico.MultiSelect = False
        Me.tabelaHistorico.Name = "tabelaHistorico"
        Me.tabelaHistorico.ReadOnly = True
        Me.tabelaHistorico.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.tabelaHistorico.RowTemplate.Height = 24
        Me.tabelaHistorico.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.tabelaHistorico.Size = New System.Drawing.Size(498, 235)
        Me.tabelaHistorico.TabIndex = 25
        '
        'histogramaImagemR
        '
        Me.histogramaImagemR.BackColor = System.Drawing.Color.WhiteSmoke
        Me.histogramaImagemR.BorderlineColor = System.Drawing.Color.WhiteSmoke
        ChartArea22.Name = "ChartArea1"
        Me.histogramaImagemR.ChartAreas.Add(ChartArea22)
        Legend22.Enabled = False
        Legend22.Name = "Legend1"
        Me.histogramaImagemR.Legends.Add(Legend22)
        Me.histogramaImagemR.Location = New System.Drawing.Point(1684, 78)
        Me.histogramaImagemR.Name = "histogramaImagemR"
        Series22.ChartArea = "ChartArea1"
        Series22.Legend = "Legend1"
        Series22.Name = "Series1"
        Me.histogramaImagemR.Series.Add(Series22)
        Me.histogramaImagemR.Size = New System.Drawing.Size(351, 255)
        Me.histogramaImagemR.TabIndex = 26
        '
        'histogramaImagemG
        '
        Me.histogramaImagemG.BackColor = System.Drawing.Color.WhiteSmoke
        ChartArea23.Name = "ChartArea1"
        Me.histogramaImagemG.ChartAreas.Add(ChartArea23)
        Legend23.Enabled = False
        Legend23.Name = "Legend1"
        Me.histogramaImagemG.Legends.Add(Legend23)
        Me.histogramaImagemG.Location = New System.Drawing.Point(1684, 323)
        Me.histogramaImagemG.Name = "histogramaImagemG"
        Series23.ChartArea = "ChartArea1"
        Series23.Legend = "Legend1"
        Series23.Name = "Series1"
        Me.histogramaImagemG.Series.Add(Series23)
        Me.histogramaImagemG.Size = New System.Drawing.Size(348, 255)
        Me.histogramaImagemG.TabIndex = 27
        Me.histogramaImagemG.Text = "Histograma"
        '
        'histogramaImagemB
        '
        Me.histogramaImagemB.BackColor = System.Drawing.Color.WhiteSmoke
        ChartArea24.Name = "ChartArea1"
        Me.histogramaImagemB.ChartAreas.Add(ChartArea24)
        Legend24.Enabled = False
        Legend24.Name = "Legend1"
        Me.histogramaImagemB.Legends.Add(Legend24)
        Me.histogramaImagemB.Location = New System.Drawing.Point(1684, 573)
        Me.histogramaImagemB.Name = "histogramaImagemB"
        Series24.ChartArea = "ChartArea1"
        Series24.Legend = "Legend1"
        Series24.Name = "Series1"
        Me.histogramaImagemB.Series.Add(Series24)
        Me.histogramaImagemB.Size = New System.Drawing.Size(348, 255)
        Me.histogramaImagemB.TabIndex = 28
        Me.histogramaImagemB.Text = "Histograma"
        '
        'dica
        '
        Me.dica.AutoPopDelay = 30000
        Me.dica.InitialDelay = 500
        Me.dica.ReshowDelay = 100
        '
        'binarizar
        '
        Me.binarizar.Location = New System.Drawing.Point(20, 91)
        Me.binarizar.Margin = New System.Windows.Forms.Padding(4)
        Me.binarizar.Name = "binarizar"
        Me.binarizar.Size = New System.Drawing.Size(193, 54)
        Me.binarizar.TabIndex = 14
        Me.binarizar.Text = "Binariza"
        Me.dica.SetToolTip(Me.binarizar, "Muda as intensidades dos componentes RGB de acordo com a intensidade de cada pixe" &
        "l da imagem em tons de cinza." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Se for <= 128, o pixel passa a ser = 0 (preto), s" &
        "enão, ele passa a ser branco(255).")
        Me.binarizar.UseVisualStyleBackColor = True
        '
        'negativo
        '
        Me.negativo.Location = New System.Drawing.Point(239, 21)
        Me.negativo.Margin = New System.Windows.Forms.Padding(4)
        Me.negativo.Name = "negativo"
        Me.negativo.Size = New System.Drawing.Size(193, 54)
        Me.negativo.TabIndex = 13
        Me.negativo.Text = "Inverte Tons"
        Me.dica.SetToolTip(Me.negativo, "Inverte as intensidades dos componentes RGB de cada pixel da imagem.")
        Me.negativo.UseVisualStyleBackColor = True
        '
        'tonsDeCinza
        '
        Me.tonsDeCinza.Location = New System.Drawing.Point(20, 21)
        Me.tonsDeCinza.Margin = New System.Windows.Forms.Padding(4)
        Me.tonsDeCinza.Name = "tonsDeCinza"
        Me.tonsDeCinza.Size = New System.Drawing.Size(193, 54)
        Me.tonsDeCinza.TabIndex = 6
        Me.tonsDeCinza.Text = "Tons de Cinza"
        Me.dica.SetToolTip(Me.tonsDeCinza, "Passa as intensidades dos componendetes RGB de cada pixel da imagem para tons de " &
        "cinza.")
        Me.tonsDeCinza.UseVisualStyleBackColor = True
        '
        'highBoost
        '
        Me.highBoost.Location = New System.Drawing.Point(20, 91)
        Me.highBoost.Margin = New System.Windows.Forms.Padding(4)
        Me.highBoost.Name = "highBoost"
        Me.highBoost.Size = New System.Drawing.Size(193, 54)
        Me.highBoost.TabIndex = 14
        Me.highBoost.Text = "Filtro High-Boost"
        Me.dica.SetToolTip(Me.highBoost, resources.GetString("highBoost.ToolTip"))
        Me.highBoost.UseVisualStyleBackColor = True
        '
        'laplace
        '
        Me.laplace.Location = New System.Drawing.Point(239, 21)
        Me.laplace.Margin = New System.Windows.Forms.Padding(4)
        Me.laplace.Name = "laplace"
        Me.laplace.Size = New System.Drawing.Size(193, 54)
        Me.laplace.TabIndex = 13
        Me.laplace.Text = "Filtro de Laplace"
        Me.dica.SetToolTip(Me.laplace, resources.GetString("laplace.ToolTip"))
        Me.laplace.UseVisualStyleBackColor = True
        '
        'sobel
        '
        Me.sobel.Location = New System.Drawing.Point(20, 21)
        Me.sobel.Margin = New System.Windows.Forms.Padding(4)
        Me.sobel.Name = "sobel"
        Me.sobel.Size = New System.Drawing.Size(193, 54)
        Me.sobel.TabIndex = 6
        Me.sobel.Text = "Filtro de Sobel"
        Me.dica.SetToolTip(Me.sobel, resources.GetString("sobel.ToolTip"))
        Me.sobel.UseVisualStyleBackColor = True
        '
        'reciclar
        '
        Me.reciclar.Location = New System.Drawing.Point(202, 12)
        Me.reciclar.Margin = New System.Windows.Forms.Padding(4)
        Me.reciclar.Name = "reciclar"
        Me.reciclar.Size = New System.Drawing.Size(125, 51)
        Me.reciclar.TabIndex = 14
        Me.reciclar.Text = "Retorna  à imagem original"
        Me.dica.SetToolTip(Me.reciclar, "Apaga o segundo quadro e coloca a imagem original sem alterações no primeiro quad" &
        "ro para ser modificada.")
        Me.reciclar.UseVisualStyleBackColor = True
        '
        'desfazer
        '
        Me.desfazer.Location = New System.Drawing.Point(104, 12)
        Me.desfazer.Margin = New System.Windows.Forms.Padding(4)
        Me.desfazer.Name = "desfazer"
        Me.desfazer.Size = New System.Drawing.Size(90, 51)
        Me.desfazer.TabIndex = 13
        Me.desfazer.Text = "Desfazer"
        Me.dica.SetToolTip(Me.desfazer, "Apaga a ultima alteração realizada e retorna à ultima guardada no histórico.")
        Me.desfazer.UseVisualStyleBackColor = True
        '
        'novaImagem
        '
        Me.novaImagem.Location = New System.Drawing.Point(8, 12)
        Me.novaImagem.Margin = New System.Windows.Forms.Padding(4)
        Me.novaImagem.Name = "novaImagem"
        Me.novaImagem.Size = New System.Drawing.Size(90, 51)
        Me.novaImagem.TabIndex = 6
        Me.novaImagem.Text = "Nova Imagem"
        Me.dica.SetToolTip(Me.novaImagem, "Insere uma nova imagem")
        Me.novaImagem.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(992, 642)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(153, 17)
        Me.Label4.TabIndex = 29
        Me.Label4.Text = "Histórico de alterações"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(23, 98)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(175, 17)
        Me.Label5.TabIndex = 30
        Me.Label5.Text = "Imagem base da alteração"
        '
        'grupoIntensidades
        '
        Me.grupoIntensidades.Controls.Add(Me.binarizar)
        Me.grupoIntensidades.Controls.Add(Me.negativo)
        Me.grupoIntensidades.Controls.Add(Me.tonsDeCinza)
        Me.grupoIntensidades.Location = New System.Drawing.Point(26, 641)
        Me.grupoIntensidades.Margin = New System.Windows.Forms.Padding(4)
        Me.grupoIntensidades.Name = "grupoIntensidades"
        Me.grupoIntensidades.Padding = New System.Windows.Forms.Padding(4)
        Me.grupoIntensidades.Size = New System.Drawing.Size(452, 161)
        Me.grupoIntensidades.TabIndex = 21
        Me.grupoIntensidades.TabStop = False
        Me.grupoIntensidades.Text = "Transformações de intensidade"
        '
        'grupoPassaAlta
        '
        Me.grupoPassaAlta.Controls.Add(Me.Label6)
        Me.grupoPassaAlta.Controls.Add(Me.pesosHighBoost)
        Me.grupoPassaAlta.Controls.Add(Me.highBoost)
        Me.grupoPassaAlta.Controls.Add(Me.laplace)
        Me.grupoPassaAlta.Controls.Add(Me.sobel)
        Me.grupoPassaAlta.Location = New System.Drawing.Point(26, 810)
        Me.grupoPassaAlta.Margin = New System.Windows.Forms.Padding(4)
        Me.grupoPassaAlta.Name = "grupoPassaAlta"
        Me.grupoPassaAlta.Padding = New System.Windows.Forms.Padding(4)
        Me.grupoPassaAlta.Size = New System.Drawing.Size(452, 161)
        Me.grupoPassaAlta.TabIndex = 21
        Me.grupoPassaAlta.TabStop = False
        Me.grupoPassaAlta.Text = "Filtros Passa-Alta"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(236, 101)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(121, 17)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Pesos High-Boost"
        '
        'pesosHighBoost
        '
        Me.pesosHighBoost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.pesosHighBoost.FormattingEnabled = True
        Me.pesosHighBoost.Items.AddRange(New Object() {"1,0", "1,2", "1,4", "1,6", "1,8", "2,0"})
        Me.pesosHighBoost.Location = New System.Drawing.Point(239, 121)
        Me.pesosHighBoost.Name = "pesosHighBoost"
        Me.pesosHighBoost.Size = New System.Drawing.Size(193, 24)
        Me.pesosHighBoost.TabIndex = 15
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.reciclar)
        Me.GroupBox4.Controls.Add(Me.desfazer)
        Me.GroupBox4.Controls.Add(Me.novaImagem)
        Me.GroupBox4.Location = New System.Drawing.Point(26, 14)
        Me.GroupBox4.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox4.Size = New System.Drawing.Size(333, 68)
        Me.GroupBox4.TabIndex = 22
        Me.GroupBox4.TabStop = False
        '
        'progressoImagem
        '
        Me.progressoImagem.Location = New System.Drawing.Point(612, 617)
        Me.progressoImagem.Name = "progressoImagem"
        Me.progressoImagem.Size = New System.Drawing.Size(463, 22)
        Me.progressoImagem.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.progressoImagem.TabIndex = 31
        '
        'janelaPse
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(1537, 990)
        Me.Controls.Add(Me.progressoImagem)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.grupoPassaAlta)
        Me.Controls.Add(Me.grupoIntensidades)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.histogramaImagemB)
        Me.Controls.Add(Me.histogramaImagemG)
        Me.Controls.Add(Me.grupoPassaBaixa)
        Me.Controls.Add(Me.histogramaImagemR)
        Me.Controls.Add(Me.tabelaHistorico)
        Me.Controls.Add(Me.grupoErosao)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.frameFoto2)
        Me.Controls.Add(Me.frameFoto1)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "janelaPse"
        Me.Text = "PSE"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.grupoPassaBaixa.ResumeLayout(False)
        Me.grupoPassaBaixa.PerformLayout()
        Me.grupoErosao.ResumeLayout(False)
        CType(Me.frameFoto2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.frameFoto1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tabelaHistorico, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.histogramaImagemR, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.histogramaImagemG, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.histogramaImagemB, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grupoIntensidades.ResumeLayout(False)
        Me.grupoPassaAlta.ResumeLayout(False)
        Me.grupoPassaAlta.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents frameFoto1 As System.Windows.Forms.PictureBox
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents media As System.Windows.Forms.Button
    Friend WithEvents frameFoto2 As System.Windows.Forms.PictureBox
    Friend WithEvents Label2 As Label
    Friend WithEvents maximo As Button
    Friend WithEvents minimo As System.Windows.Forms.Button
    Friend WithEvents mediana As System.Windows.Forms.Button
    Friend WithEvents grupoPassaBaixa As System.Windows.Forms.GroupBox
    Friend WithEvents grupoErosao As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents tabelaHistorico As DataGridView
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
    Friend WithEvents Label3 As Label
    Friend WithEvents histogramaImagemR As DataVisualization.Charting.Chart
    Friend WithEvents histogramaImagemG As DataVisualization.Charting.Chart
    Friend WithEvents histogramaImagemB As DataVisualization.Charting.Chart
    Friend WithEvents dica As ToolTip
    Friend WithEvents Label4 As Label
    Friend WithEvents gaussiano As Button
    Friend WithEvents Label5 As Label
    Friend WithEvents grupoIntensidades As GroupBox
    Friend WithEvents binarizar As Button
    Friend WithEvents negativo As Button
    Friend WithEvents tonsDeCinza As Button
    Friend WithEvents grupoPassaAlta As GroupBox
    Friend WithEvents highBoost As Button
    Friend WithEvents laplace As Button
    Friend WithEvents sobel As Button
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents reciclar As Button
    Friend WithEvents desfazer As Button
    Friend WithEvents novaImagem As Button
    Friend WithEvents Label6 As Label
    Friend WithEvents pesosHighBoost As ComboBox
    Friend WithEvents Label7 As Label
    Friend WithEvents dimensoesMascara As ComboBox
    Friend WithEvents fechamento As Button
    Friend WithEvents abertura As Button
    Friend WithEvents progressoImagem As ProgressBar
End Class
