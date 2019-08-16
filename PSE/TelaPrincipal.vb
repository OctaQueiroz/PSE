Public Class janelaPse
    '-----------------------------------------------------------------------------------------------------------------------------------------------------
    '-----------------------------------VARIÁVEIS GLOBAIS-------------------------------------------------------------------------------------------------

    'Variável que verifica se a imagem foi alterada, se sim, a proxima alteração será feita na imagem do frame 2 não no frame 1
    Dim imagemAlterada As Boolean

    'Variável que monitora o ID da última alteração adicionada ao histórico
    Dim idImagens As Integer

    'Guarda a imagem em seu tamanho original. Todas as alterações são realizadas nessa variável e a mesma é redimensionada para caber nos frames
    Dim imagemOriginal As Image

    'Vetor de intensidades para mapear o eixo X do histograma
    Dim intensidades(256) As Integer

    'Lista de histogramas para guardar o histórico de histogramas das alterações
    Dim historicoHistograma As List(Of Histograma) = New List(Of Histograma)

    '-----------------------------------------------------------------------------------------------------------------------------------------------------

    'Configurações de inicialização do programa
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        OpenFileDialog1.RestoreDirectory = True 'Autoriza o registro do último diretório acessado, e abre nele na proxima utilização
        OpenFileDialog1.Title = "Selecione a imagem" 'Título do frame
        OpenFileDialog1.DefaultExt = "jpg" 'Define o tipo padrão de arquivo
        OpenFileDialog1.Filter = "jpg files (*.jpg)|*.jpg|png files (*.png)|*.png|jpeg files (*.jpeg)|*.jpeg|All files (*.*)|*.*" 'Define o filtro de arquivos que serão mostrados na pesquisa
        OpenFileDialog1.CheckFileExists = True 'Motra alerta se o usuário inseriu o nome de um arquivo existente
        OpenFileDialog1.CheckPathExists = True 'Motra alerta se o usuário inseriu um caminho existente

        'Variável para criar a coluna de imagens na tabela de histórico
        Dim imagens As New DataGridViewImageColumn()

        'Esconde a barra de progresso
        progressoImagem.Visible = False

        'Inicializa colunas da tabela
        tabelaHistorico.Columns.Add("id", "idImagem")
        tabelaHistorico.Columns.Add("alteracao", "Alteração Realizada")
        tabelaHistorico.Columns.Add(imagens)
        tabelaHistorico.Columns(1).Width = 130

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

        'Inicializa os histogramas
        Dim dadosNovos As Histograma = New Histograma()
        histogramaImagemR.Series(0).Points.DataBindXY(intensidades, dadosNovos.quantidadePorIntensidadeR)
        histogramaImagemG.Series(0).Points.DataBindXY(intensidades, dadosNovos.quantidadePorIntensidadeG)
        histogramaImagemB.Series(0).Points.DataBindXY(intensidades, dadosNovos.quantidadePorIntensidadeB)

    End Sub

    '-----------------------------------------------------------------------------------------------------------------------------------------------------
    '-----------------------------------BOTÕES DE ALTERAÇÃO DE ESTADO DA IMAGEM---------------------------------------------------------------------------

    'Botão para fazer a chamada da busca de imagem no explorer
    Private Sub novaImagem_Click(sender As Object, e As EventArgs) Handles novaImagem.Click
        Dim bmp As Bitmap

        If OpenFileDialog1.ShowDialog <> DialogResult.Cancel Then
            If (tabelaHistorico.Rows.Count > 1) Then
                If (MessageBox.Show("Adicionar uma nova imagem apagará todo o histórico de alterações realizado na imagem atual. Tem certeza que deseja continuar?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Information) <> DialogResult.No) Then

                    'Desativa os botões da tela para evitar escalonamento de atividades
                    desativaBotões()

                    'Limpa o histórico
                    tabelaHistorico.Rows.Clear()

                    'Limpa o histórico de histogramas
                    historicoHistograma = New List(Of Histograma)
                    idImagens = 0

                    'Pega a nova imagem à partir do seu caminho de dados
                    Dim img As String = OpenFileDialog1.FileName
                    bmp = Bitmap.FromFile(img)
                    Dim bmpTemp As New Bitmap(frameFoto1.Width, frameFoto1.Height)

                    'Guarda a imagem original
                    imagemOriginal = bmp

                    'Reduz o tamanho da imagem original caso ela seja maior que Full HD. 
                    'Isso deve ser feito para evitar um grande custo desnecessário de processamento, visto que não haverá perda  significativa
                    'de detalhes com esta redução
                    If (imagemOriginal.Height > 1080 Or imagemOriginal.Width > 1920) Then
                        imagemOriginal = reduzTamanhoImagem(imagemOriginal)
                    End If

                    frameFoto1.Image = redimensiona(imagemOriginal)
                    frameFoto2.Image = Nothing

                    'Mantém a imagem original na tabela
                    tabelaHistorico.Rows.Add("Original", "Nenhuma", imagemOriginal)
                    idImagens += 1

                    Dim dadosImagem As Histograma

                    'Ativa a barra de progresso
                    progressoImagem.Visible = True
                    'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                    progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height

                    'Calcula o histograma da imagem
                    dadosImagem = quantizaIntensidades(imagemOriginal)

                    'guarda o histograma no histórico
                    historicoHistograma.Add(dadosImagem)

                    'Gera o histograma da imagem para a as três intensidades
                    histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                    histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                    histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                    imagemAlterada = False

                    'Desativa a barra de progresso
                    progressoImagem.Visible = False
                    progressoImagem.Value = 0

                    'Reativa os botões da tela
                    ativaBotões()
                End If
            Else

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                'Pega a nova imagem à partir do seu caminho de dados
                Dim img As String = OpenFileDialog1.FileName
                bmp = Bitmap.FromFile(img)
                Dim bmpTemp As New Bitmap(frameFoto1.Width, frameFoto1.Height)

                'Guarda a imagem original
                imagemOriginal = bmp

                'Reduz o tamanho da imagem original caso ela seja maior que Full HD. 
                'Isso deve ser feito para evitar um grande custo desnecessário de processamento, visto que não haverá perda  significativa
                'de detalhes com esta redução
                If (imagemOriginal.Height > 1080 Or imagemOriginal.Width > 1920) Then
                    imagemOriginal = reduzTamanhoImagem(imagemOriginal)
                End If

                frameFoto1.Image = redimensiona(imagemOriginal)

                'Mantém a imagem original na tabela
                tabelaHistorico.Rows.Add("Original", "Nenhuma", imagemOriginal)
                idImagens += 1

                Dim dadosImagem As Histograma

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height

                'Calcula o histograma da imagem
                dadosImagem = quantizaIntensidades(imagemOriginal)

                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                imagemAlterada = False

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            End If
        End If
    End Sub

    'Botão para desfazer  ultima alteração realizada
    Private Sub desfazer_Click(sender As Object, e As EventArgs) Handles desfazer.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            'Deleta a última imagem salva no histórico
            tabelaHistorico.Rows.RemoveAt(tabelaHistorico.Rows.Count - 2)
            frameFoto1.Image = Nothing
            frameFoto2.Image = Nothing

            'Deleta o último histograma salvo no histórico de histogramas
            historicoHistograma.RemoveAt(historicoHistograma.Count - 1)

            Dim dadosNovos As Histograma = New Histograma()
            'Apaga o histograma das 3 intensidades
            histogramaImagemR.Series(0).Points.DataBindXY(intensidades, dadosNovos.quantidadePorIntensidadeR)
            histogramaImagemG.Series(0).Points.DataBindXY(intensidades, dadosNovos.quantidadePorIntensidadeG)
            histogramaImagemB.Series(0).Points.DataBindXY(intensidades, dadosNovos.quantidadePorIntensidadeB)

            'Decresce a quantidade de imagens no histórico
            idImagens -= 1

            'Caso a  última imagem removida não tenha sido a única do histórico, coloca a última imagem do histórico no quadro da original 
            If (tabelaHistorico.Rows.Count > 1) Then
                frameFoto1.Image = redimensiona(tabelaHistorico.Rows(tabelaHistorico.Rows.Count - 2).Cells(2).Value)
                imagemOriginal = tabelaHistorico.Rows(tabelaHistorico.Rows.Count - 2).Cells(2).Value

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

            End If

            'Seta como falso para que a prixima alteração seja feita na imagem do primeiro quadro, não do segundo
            imagemAlterada = False
        Else
            MessageBox.Show("Não há alterações para serem desfeitas", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    'Botão para retornar a imagem alterada para a original sem apagar históricos de alteração
    Private Sub retornaImagemInicial_Click(sender As Object, e As EventArgs) Handles reciclar.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            frameFoto1.Image = redimensiona(tabelaHistorico.Rows(0).Cells(2).Value)
            imagemOriginal = tabelaHistorico.Rows(0).Cells(2).Value

            'Retorna a foto original ao histórico
            tabelaHistorico.Rows.Add(CStr(idImagens), "Retorna à original", imagemOriginal)
            idImagens += 1

            'Apaga o conteúdo do segundo quadro
            frameFoto2.Image = Nothing

            'Guarda o histograma da imagem inicial no histórico, é uam redundância para manter a coerencia entre a tabela de imagens e a lista de histogramas
            historicoHistograma.Add(historicoHistograma(0))

            'Gera o histograma da imagem para a as três intensidades
            histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(0).quantidadePorIntensidadeR)
            histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(0).quantidadePorIntensidadeG)
            histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(0).quantidadePorIntensidadeB)

            'Seta como falso para que a próxima alteração seja feita na imagem do primeiro quadro, não do segundo
            imagemAlterada = False
        Else
            MessageBox.Show("Não há nenhuma imagem no histórico", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    'Botão de chamada para visualização do histórico de imagem e histogramas
    Private Sub tabelaHistorico_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles tabelaHistorico.CellDoubleClick
        If (tabelaHistorico.Rows.Count > 1) Then

            'Passa como parâmetro a imagem e o ID concatenado à descrição da alteração realizada  na imagem
            Dim visualizacaoImagem As TelaVisualizacaoImagens = New TelaVisualizacaoImagens(tabelaHistorico.CurrentRow.Cells(2).Value, (tabelaHistorico.CurrentRow.Cells(0).Value.ToString + "_" + tabelaHistorico.CurrentRow.Cells(1).Value.ToString))

            Dim visualizacaoHistograma As TelaVisualizacaoHistograma

            If tabelaHistorico.CurrentRow.Cells(0).Value.ToString.Equals("Original") Then

                'Passa como parâmetro a imagem e o ID concatenado à descrição da alteração realizada  na imagem
                visualizacaoHistograma = New TelaVisualizacaoHistograma(historicoHistograma(0), (tabelaHistorico.CurrentRow.Cells(0).Value.ToString + "_" + tabelaHistorico.CurrentRow.Cells(1).Value.ToString))
            Else

                'Passa como parâmetro a imagem e o ID concatenado à descrição da alteração realizada  na imagem
                visualizacaoHistograma = New TelaVisualizacaoHistograma(historicoHistograma(tabelaHistorico.CurrentRow.Cells(0).Value), (tabelaHistorico.CurrentRow.Cells(0).Value.ToString + "_" + tabelaHistorico.CurrentRow.Cells(1).Value.ToString))
            End If

            visualizacaoImagem.Show()
            visualizacaoHistograma.Show()
        End If
    End Sub

    '-----------------------------------------------------------------------------------------------------------------------------------------------------
    '-----------------------------------BOTÕES DE TRANSFORMAÇÃO DE INTENSIDADE----------------------------------------------------------------------------

    'Botão responsável por realizar a chamada para a função de transformação para tons de cinza
    Private Sub tonsCinza_Click(sender As Object, e As EventArgs) Handles tonsDeCinza.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            If Not (imagemAlterada) Then

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                'Ativa a barra de progresso
                progressoImagem.Visible = True

                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height

                imagemOriginal = tomDeCinza(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                tabelaHistorico.Rows.Add(CStr(idImagens), "Tom de cinza", imagemOriginal)
                idImagens += 1
                imagemAlterada = True

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()

            Else

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                frameFoto1.Image = frameFoto2.Image

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = tomDeCinza(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Tom de cinza", imagemOriginal)
                idImagens += 1

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            End If
        Else
            MessageBox.Show("Não há nenhuma imagem para ser alterada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    'Botão de chamada para a função de inversão de tons de RGB da imagem
    Private Sub negativo_Click(sender As Object, e As EventArgs) Handles negativo.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            If Not (imagemAlterada) Then

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = inverteTons(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Negativo", imagemOriginal)
                idImagens += 1
                imagemAlterada = True

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)
                 
                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            Else

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                frameFoto1.Image = frameFoto2.Image

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = inverteTons(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Negativo", imagemOriginal)
                idImagens += 1

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            End If
        Else
            MessageBox.Show("Não há nenhuma imagem para ser alterada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    'Botão de chamada para a função de binarização
    Private Sub binarizar_Click(sender As Object, e As EventArgs) Handles binarizar.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            If Not (imagemAlterada) Then

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + imagemOriginal.Width * imagemOriginal.Height

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = binariza(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Binarização", imagemOriginal)
                idImagens += 1
                imagemAlterada = True

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            Else

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                Dim dadosImagem As New Histograma
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                frameFoto1.Image = frameFoto2.Image

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + imagemOriginal.Width * imagemOriginal.Height

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = binariza(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Binarização", imagemOriginal)
                idImagens += 1

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            End If
        Else
            MessageBox.Show("Não há nenhuma imagem para ser alterada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    '-----------------------------------------------------------------------------------------------------------------------------------------------------
    '-----------------------------------BOTÕES DE FILTROS PASSA-ALTAS-------------------------------------------------------------------------------------

    'Botão de chamada para a função de filtro de Sobel
    Private Sub sobel_Click(sender As Object, e As EventArgs) Handles sobel.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            If Not (imagemAlterada) Then

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + imagemOriginal.Width * imagemOriginal.Height

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = filtroSobel(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro de Sobel", imagemOriginal)
                idImagens += 1
                imagemAlterada = True

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            Else

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                frameFoto1.Image = frameFoto2.Image

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + imagemOriginal.Width * imagemOriginal.Height

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = filtroSobel(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro de Sobel", imagemOriginal)
                idImagens += 1

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            End If
        Else
            MessageBox.Show("Não há nenhuma imagem para ser alterada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    'Botão de chamada para a função de filtro Laplaciano
    Private Sub laplace_Click(sender As Object, e As EventArgs) Handles laplace.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            If Not (imagemAlterada) Then

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + imagemOriginal.Width * imagemOriginal.Height

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = filtroLaplaciano(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro de Laplace", imagemOriginal)
                idImagens += 1
                imagemAlterada = True

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            Else

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                frameFoto1.Image = frameFoto2.Image

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + imagemOriginal.Width * imagemOriginal.Height

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = filtroLaplaciano(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro de Laplace", imagemOriginal)
                idImagens += 1

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            End If
        Else
            MessageBox.Show("Não há nenhuma imagem para ser alterada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    'Botão de chamada para a função de filtro High-Boost
    Private Sub high_boost_Click(sender As Object, e As EventArgs) Handles highBoost.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            If (pesosHighBoost.SelectedItem <> Nothing) Then
                If Not (imagemAlterada) Then

                    'Desativa os botões da tela para evitar escalonamento de atividades
                    desativaBotões()

                    Dim dadosImagem As Histograma = New Histograma()
                    'guarda o histograma no histórico
                    historicoHistograma.Add(dadosImagem)

                    'Ativa a barra de progresso
                    progressoImagem.Visible = True
                    'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                    progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height

                    'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                    imagemOriginal = filtroHighBoost(imagemOriginal, CDbl(pesosHighBoost.SelectedItem))
                    frameFoto2.Image = redimensiona(imagemOriginal)

                    'Adiciona a nova alteração ao histórico de imagens
                    tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro High-Boost" + CStr(pesosHighBoost.SelectedItem), imagemOriginal)
                    idImagens += 1
                    imagemAlterada = True

                    'Gera o histograma da imagem para a as três intensidades
                    histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                    histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                    histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                    'Desativa a barra de progresso
                    progressoImagem.Visible = False
                    progressoImagem.Value = 0

                    'Reativa os botões da tela
                    ativaBotões()
                Else

                    'Desativa os botões da tela para evitar escalonamento de atividades
                    desativaBotões()

                    Dim dadosImagem As Histograma = New Histograma()
                    'guarda o histograma no histórico
                    historicoHistograma.Add(dadosImagem)

                    frameFoto1.Image = frameFoto2.Image

                    'Ativa a barra de progresso
                    progressoImagem.Visible = True
                    'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                    progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height

                    'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                    imagemOriginal = filtroHighBoost(imagemOriginal, pesosHighBoost.SelectedItem)
                    frameFoto2.Image = redimensiona(imagemOriginal)

                    'Adiciona a nova alteração ao histórico de imagens
                    tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro High-Boost" + CStr(pesosHighBoost.SelectedItem), imagemOriginal)
                    idImagens += 1

                    'Gera o histograma da imagem para a as três intensidades
                    histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                    histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                    histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                    'Desativa a barra de progresso
                    progressoImagem.Visible = False
                    progressoImagem.Value = 0

                    'Reativa os botões da tela
                    ativaBotões()
                End If
            Else
                MessageBox.Show("Valor de peso deve ser numeral e >= 1", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Else
            MessageBox.Show("Não há nenhuma imagem para ser alterada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    '-----------------------------------------------------------------------------------------------------------------------------------------------------
    '-----------------------------------BOTÕES DE FILTROS PASSA-BAIXAS------------------------------------------------------------------------------------

    'Botão de chamada para a função de filtro de média
    Private Sub media_Click(sender As Object, e As EventArgs) Handles media.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            If (dimensoesMascara.SelectedItem <> Nothing) Then
                If Not (imagemAlterada) Then
                    If (dimensoesMascara.SelectedItem.ToString.Equals("3x3")) Then

                        'Desativa os botões da tela para evitar escalonamento de atividades
                        desativaBotões()

                        Dim dadosImagem As Histograma = New Histograma()
                        'guarda o histograma no histórico
                        historicoHistograma.Add(dadosImagem)

                        'Ativa a barra de progresso
                        progressoImagem.Visible = True
                        'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                        progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 2) * (imagemOriginal.Height + 2))

                        'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                        imagemOriginal = filtroMedia3x3(imagemOriginal)
                        frameFoto2.Image = redimensiona(imagemOriginal)

                        'Adiciona a nova alteração ao histórico de imagens
                        tabelaHistorico.Rows.Add(CStr(idImagens), "3x3 Filtro de Média", imagemOriginal)
                        idImagens += 1
                        imagemAlterada = True

                        'Gera o histograma da imagem para a as três intensidades
                        histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                        histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                        histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                        'Desativa a barra de progresso
                        progressoImagem.Visible = False
                        progressoImagem.Value = 0

                        'Reativa os botões da tela
                        ativaBotões()
                    Else
                        If (dimensoesMascara.SelectedItem.ToString.Equals("5x5")) Then

                            'Desativa os botões da tela para evitar escalonamento de atividades
                            desativaBotões()

                            Dim dadosImagem As Histograma = New Histograma()
                            'guarda o histograma no histórico
                            historicoHistograma.Add(dadosImagem)

                            'Ativa a barra de progresso
                            progressoImagem.Visible = True
                            'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                            progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 4) * (imagemOriginal.Height + 4))

                            'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                            imagemOriginal = filtroMedia5x5(imagemOriginal)
                            frameFoto2.Image = redimensiona(imagemOriginal)

                            'Adiciona a nova alteração ao histórico de imagens
                            tabelaHistorico.Rows.Add(CStr(idImagens), "5x5 Filtro de Média", imagemOriginal)
                            idImagens += 1
                            imagemAlterada = True

                            'Gera o histograma da imagem para a as três intensidades
                            histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                            histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                            histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                            'Desativa a barra de progresso
                            progressoImagem.Visible = False
                            progressoImagem.Value = 0

                            'Reativa os botões da tela
                            ativaBotões()
                        Else
                            If (dimensoesMascara.SelectedItem.ToString.Equals("7x7")) Then

                                'Desativa os botões da tela para evitar escalonamento de atividades
                                desativaBotões()

                                Dim dadosImagem As Histograma = New Histograma()
                                'guarda o histograma no histórico
                                historicoHistograma.Add(dadosImagem)

                                'Ativa a barra de progresso
                                progressoImagem.Visible = True
                                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 6) * (imagemOriginal.Height + 6))
                                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                                imagemOriginal = filtroMedia7x7(imagemOriginal)
                                frameFoto2.Image = redimensiona(imagemOriginal)

                                'Adiciona a nova alteração ao histórico de imagens
                                tabelaHistorico.Rows.Add(CStr(idImagens), "7x7 Filtro de Média", imagemOriginal)
                                idImagens += 1
                                imagemAlterada = True

                                'Gera o histograma da imagem para a as três intensidades
                                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                                'Desativa a barra de progresso
                                progressoImagem.Visible = False
                                progressoImagem.Value = 0

                                'Reativa os botões da tela
                                ativaBotões()
                            End If
                        End If
                    End If
                Else
                    If (dimensoesMascara.SelectedItem.ToString.Equals("3x3")) Then

                        'Desativa os botões da tela para evitar escalonamento de atividades
                        desativaBotões()

                        Dim dadosImagem As Histograma = New Histograma()
                        'guarda o histograma no histórico
                        historicoHistograma.Add(dadosImagem)

                        frameFoto1.Image = frameFoto2.Image

                        'Ativa a barra de progresso
                        progressoImagem.Visible = True
                        'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                        progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 2) * (imagemOriginal.Height + 2))

                        'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                        imagemOriginal = filtroMedia3x3(imagemOriginal)
                        frameFoto2.Image = redimensiona(imagemOriginal)

                        'Adiciona a nova alteração ao histórico de imagens
                        tabelaHistorico.Rows.Add(CStr(idImagens), "3x3 Filtro de Média", imagemOriginal)
                        idImagens += 1

                        'Gera o histograma da imagem para a as três intensidades
                        histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                        histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                        histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                        'Desativa a barra de progresso
                        progressoImagem.Visible = False
                        progressoImagem.Value = 0

                        'Reativa os botões da tela
                        ativaBotões()
                    Else
                        If (dimensoesMascara.SelectedItem.ToString.Equals("5x5")) Then

                            'Desativa os botões da tela para evitar escalonamento de atividades
                            desativaBotões()

                            Dim dadosImagem As Histograma = New Histograma()
                            'guarda o histograma no histórico
                            historicoHistograma.Add(dadosImagem)

                            frameFoto1.Image = frameFoto2.Image

                            'Ativa a barra de progresso
                            progressoImagem.Visible = True
                            'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                            progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 4) * (imagemOriginal.Height + 4))

                            'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                            imagemOriginal = filtroMedia5x5(imagemOriginal)
                            frameFoto2.Image = redimensiona(imagemOriginal)

                            'Adiciona a nova alteração ao histórico de imagens
                            tabelaHistorico.Rows.Add(CStr(idImagens), "5x5 Filtro de Média", imagemOriginal)
                            idImagens += 1

                            'Gera o histograma da imagem para a as três intensidades
                            histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                            histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                            histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                            'Desativa a barra de progresso
                            progressoImagem.Visible = False
                            progressoImagem.Value = 0

                            'Reativa os botões da tela
                            ativaBotões()
                        Else
                            If (dimensoesMascara.SelectedItem.ToString.Equals("7x7")) Then

                                'Desativa os botões da tela para evitar escalonamento de atividades
                                desativaBotões()

                                Dim dadosImagem As Histograma = New Histograma()
                                'guarda o histograma no histórico
                                historicoHistograma.Add(dadosImagem)

                                frameFoto1.Image = frameFoto2.Image

                                'Ativa a barra de progresso
                                progressoImagem.Visible = True
                                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 6) * (imagemOriginal.Height + 6))

                                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                                imagemOriginal = filtroMedia7x7(imagemOriginal)
                                frameFoto2.Image = redimensiona(imagemOriginal)

                                'Adiciona a nova alteração ao histórico de imagens
                                tabelaHistorico.Rows.Add(CStr(idImagens), "7x7 Filtro de Média", imagemOriginal)
                                idImagens += 1

                                'Gera o histograma da imagem para a as três intensidades
                                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                                'Desativa a barra de progresso
                                progressoImagem.Visible = False
                                progressoImagem.Value = 0

                                'Reativa os botões da tela
                                ativaBotões()
                            End If
                        End If
                    End If
                End If
            Else
                MessageBox.Show("Selecione um tamanho para a máscara", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Else
            MessageBox.Show("Não há nenhuma imagem para ser alterada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    'Botão de chamada para a função de filtro de Mediana
    Private Sub mediana_Click(sender As Object, e As EventArgs) Handles mediana.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            If (dimensoesMascara.SelectedItem <> Nothing) Then
                If Not (imagemAlterada) Then
                    If (dimensoesMascara.SelectedItem.ToString.Equals("3x3")) Then

                        'Desativa os botões da tela para evitar escalonamento de atividades
                        desativaBotões()

                        Dim dadosImagem As Histograma = New Histograma()
                        'guarda o histograma no histórico
                        historicoHistograma.Add(dadosImagem)

                        'Ativa a barra de progresso
                        progressoImagem.Visible = True
                        'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                        progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 2) * (imagemOriginal.Height + 2)) 'Multiplica pelo numero de operações da função de ordenação, que no caso é (3x3)²

                        'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                        imagemOriginal = filtroMediana3x3(imagemOriginal)
                        frameFoto2.Image = redimensiona(imagemOriginal)

                        'Adiciona a nova alteração ao histórico de imagens
                        tabelaHistorico.Rows.Add(CStr(idImagens), "3x3 Filtro de Mediana", imagemOriginal)
                        idImagens += 1
                        imagemAlterada = True

                        'Gera o histograma da imagem para a as três intensidades
                        histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                        histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                        histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                        'Desativa a barra de progresso
                        progressoImagem.Visible = False
                        progressoImagem.Value = 0

                        'Reativa os botões da tela
                        ativaBotões()
                    Else
                        If (dimensoesMascara.SelectedItem.ToString.Equals("5x5")) Then

                            'Desativa os botões da tela para evitar escalonamento de atividades
                            desativaBotões()

                            Dim dadosImagem As Histograma = New Histograma()
                            'guarda o histograma no histórico
                            historicoHistograma.Add(dadosImagem)

                            'Ativa a barra de progresso
                            progressoImagem.Visible = True
                            'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                            progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 4) * (imagemOriginal.Height + 4)) 'Multiplica pelo numero de operações da função de ordenação, que no caso é (5x5)²

                            'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                            imagemOriginal = filtroMediana5x5(imagemOriginal)
                            frameFoto2.Image = redimensiona(imagemOriginal)

                            'Adiciona a nova alteração ao histórico de imagens
                            tabelaHistorico.Rows.Add(CStr(idImagens), "5x5 Filtro de Mediana", imagemOriginal)
                            idImagens += 1
                            imagemAlterada = True

                            'Gera o histograma da imagem para a as três intensidades
                            histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                            histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                            histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                            'Desativa a barra de progresso
                            progressoImagem.Visible = False
                            progressoImagem.Value = 0

                            'Reativa os botões da tela
                            ativaBotões()
                        Else
                            If (dimensoesMascara.SelectedItem.ToString.Equals("7x7")) Then

                                'Desativa os botões da tela para evitar escalonamento de atividades
                                desativaBotões()

                                Dim dadosImagem As Histograma = New Histograma()
                                'guarda o histograma no histórico
                                historicoHistograma.Add(dadosImagem)

                                'Ativa a barra de progresso
                                progressoImagem.Visible = True
                                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 6) * (imagemOriginal.Height + 6)) 'Multiplica pelo numero de operações da função de ordenação, que no caso é (7x7)²

                                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                                imagemOriginal = filtroMediana7x7(imagemOriginal)
                                frameFoto2.Image = redimensiona(imagemOriginal)

                                'Adiciona a nova alteração ao histórico de imagens
                                tabelaHistorico.Rows.Add(CStr(idImagens), "7x7 Filtro de Mediana", imagemOriginal)
                                idImagens += 1
                                imagemAlterada = True

                                'Gera o histograma da imagem para a as três intensidades
                                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                                'Desativa a barra de progresso
                                progressoImagem.Visible = False
                                progressoImagem.Value = 0

                                'Reativa os botões da tela
                                ativaBotões()
                            End If
                        End If
                    End If

                Else
                    If (dimensoesMascara.SelectedItem.ToString.Equals("3x3")) Then

                        'Desativa os botões da tela para evitar escalonamento de atividades
                        desativaBotões()

                        Dim dadosImagem As Histograma = New Histograma()
                        'guarda o histograma no histórico
                        historicoHistograma.Add(dadosImagem)

                        frameFoto1.Image = frameFoto2.Image

                        'Ativa a barra de progresso
                        progressoImagem.Visible = True
                        'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                        progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 2) * (imagemOriginal.Height + 2)) 'Multiplica pelo numero de operações da função de ordenação, que no caso é (3x3)²

                        'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                        imagemOriginal = filtroMediana3x3(imagemOriginal)
                        frameFoto2.Image = redimensiona(imagemOriginal)

                        'Adiciona a nova alteração ao histórico de imagens
                        tabelaHistorico.Rows.Add(CStr(idImagens), "3x3 Filtro de Mediana", imagemOriginal)
                        idImagens += 1

                        'Gera o histograma da imagem para a as três intensidades
                        histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                        histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                        histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                        'Desativa a barra de progresso
                        progressoImagem.Visible = False
                        progressoImagem.Value = 0

                        'Reativa os botões da tela
                        ativaBotões()
                    Else
                        If (dimensoesMascara.SelectedItem.ToString.Equals("5x5")) Then

                            'Desativa os botões da tela para evitar escalonamento de atividades
                            desativaBotões()

                            Dim dadosImagem As Histograma = New Histograma()
                            'guarda o histograma no histórico
                            historicoHistograma.Add(dadosImagem)

                            frameFoto1.Image = frameFoto2.Image

                            'Ativa a barra de progresso
                            progressoImagem.Visible = True
                            'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                            progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 4) * (imagemOriginal.Height + 4)) 'Multiplica pelo numero de operações da função de ordenação, que no caso é (5x5)²

                            'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                            imagemOriginal = filtroMediana5x5(imagemOriginal)
                            frameFoto2.Image = redimensiona(imagemOriginal)

                            'Adiciona a nova alteração ao histórico de imagens
                            tabelaHistorico.Rows.Add(CStr(idImagens), "5x5 Filtro de Mediana", imagemOriginal)
                            idImagens += 1

                            'Gera o histograma da imagem para a as três intensidades
                            histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                            histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                            histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                            'Desativa a barra de progresso
                            progressoImagem.Visible = False
                            progressoImagem.Value = 0

                            'Reativa os botões da tela
                            ativaBotões()
                        Else
                            If (dimensoesMascara.SelectedItem.ToString.Equals("7x7")) Then

                                'Desativa os botões da tela para evitar escalonamento de atividades
                                desativaBotões()

                                Dim dadosImagem As Histograma = New Histograma()
                                'guarda o histograma no histórico
                                historicoHistograma.Add(dadosImagem)

                                frameFoto1.Image = frameFoto2.Image

                                'Ativa a barra de progresso
                                progressoImagem.Visible = True
                                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 6) * (imagemOriginal.Height + 6)) 'Multiplica pelo numero de operações da função de ordenação, que no caso é (7x7)²

                                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                                imagemOriginal = filtroMediana7x7(imagemOriginal)
                                frameFoto2.Image = redimensiona(imagemOriginal)

                                'Adiciona a nova alteração ao histórico de imagens
                                tabelaHistorico.Rows.Add(CStr(idImagens), "7x7 Filtro de Mediana", imagemOriginal)
                                idImagens += 1

                                'Gera o histograma da imagem para a as três intensidades
                                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                                'Desativa a barra de progresso
                                progressoImagem.Visible = False
                                progressoImagem.Value = 0

                                'Reativa os botões da tela
                                ativaBotões()
                            End If
                        End If
                    End If
                End If
            Else
                MessageBox.Show("Selecione um tamanho para a máscara", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Else
            MessageBox.Show("Não há nenhuma imagem para ser alterada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub



    'Botão de chamada para a função de filtro Gaussiano
    Private Sub gaussiano_Click(sender As Object, e As EventArgs) Handles gaussiano.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            If (dimensoesMascara.SelectedItem <> Nothing) Then
                If Not (imagemAlterada) Then
                    If (dimensoesMascara.SelectedItem.ToString.Equals("3x3")) Then

                        'Desativa os botões da tela para evitar escalonamento de atividades
                        desativaBotões()

                        Dim dadosImagem As Histograma = New Histograma()
                        'guarda o histograma no histórico
                        historicoHistograma.Add(dadosImagem)

                        'Ativa a barra de progresso
                        progressoImagem.Visible = True
                        'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                        progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 2) * (imagemOriginal.Height + 2))

                        'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                        imagemOriginal = filtroGaussiano3x3(imagemOriginal)
                        frameFoto2.Image = redimensiona(imagemOriginal)

                        'Adiciona a nova alteração ao histórico de imagens
                        tabelaHistorico.Rows.Add(CStr(idImagens), "3x3 Filtro Gaussiano", imagemOriginal)
                        idImagens += 1
                        imagemAlterada = True

                        'Gera o histograma da imagem para a as três intensidades
                        histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                        histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                        histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                        'Desativa a barra de progresso
                        progressoImagem.Visible = False
                        progressoImagem.Value = 0

                        'Reativa os botões da tela
                        ativaBotões()
                    Else
                        If (dimensoesMascara.SelectedItem.ToString.Equals("5x5")) Then

                            'Desativa os botões da tela para evitar escalonamento de atividades
                            desativaBotões()

                            Dim dadosImagem As Histograma = New Histograma()
                            'guarda o histograma no histórico
                            historicoHistograma.Add(dadosImagem)

                            'Ativa a barra de progresso
                            progressoImagem.Visible = True
                            'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                            progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 4) * (imagemOriginal.Height + 4))

                            'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                            imagemOriginal = filtroGaussiano5x5(imagemOriginal)
                            frameFoto2.Image = redimensiona(imagemOriginal)

                            'Adiciona a nova alteração ao histórico de imagens
                            tabelaHistorico.Rows.Add(CStr(idImagens), "5x5 Filtro Gaussiano", imagemOriginal)
                            idImagens += 1
                            imagemAlterada = True

                            'Gera o histograma da imagem para a as três intensidades
                            histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                            histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                            histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                            'Desativa a barra de progresso
                            progressoImagem.Visible = False
                            progressoImagem.Value = 0

                            'Reativa os botões da tela
                            ativaBotões()
                        Else
                            If (dimensoesMascara.SelectedItem.ToString.Equals("7x7")) Then

                                'Desativa os botões da tela para evitar escalonamento de atividades
                                desativaBotões()

                                Dim dadosImagem As Histograma = New Histograma()
                                'guarda o histograma no histórico
                                historicoHistograma.Add(dadosImagem)

                                'Ativa a barra de progresso
                                progressoImagem.Visible = True
                                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 6) * (imagemOriginal.Height + 6))

                                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                                imagemOriginal = filtroGaussiano7x7(imagemOriginal)
                                frameFoto2.Image = redimensiona(imagemOriginal)

                                'Adiciona a nova alteração ao histórico de imagens
                                tabelaHistorico.Rows.Add(CStr(idImagens), "7x7 Filtro Gaussiano", imagemOriginal)
                                idImagens += 1
                                imagemAlterada = True

                                'Gera o histograma da imagem para a as três intensidades
                                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                                'Desativa a barra de progresso
                                progressoImagem.Visible = False
                                progressoImagem.Value = 0

                                'Reativa os botões da tela
                                ativaBotões()
                            End If
                        End If
                    End If
                Else
                    If (dimensoesMascara.SelectedItem.ToString.Equals("3x3")) Then

                        'Desativa os botões da tela para evitar escalonamento de atividades
                        desativaBotões()

                        Dim dadosImagem As Histograma = New Histograma()
                        'guarda o histograma no histórico
                        historicoHistograma.Add(dadosImagem)

                        frameFoto1.Image = frameFoto2.Image

                        'Ativa a barra de progresso
                        progressoImagem.Visible = True
                        'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                        progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 2) * (imagemOriginal.Height + 2))

                        'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                        imagemOriginal = filtroGaussiano3x3(imagemOriginal)
                        frameFoto2.Image = redimensiona(imagemOriginal)

                        'Adiciona a nova alteração ao histórico de imagens
                        tabelaHistorico.Rows.Add(CStr(idImagens), "3x3 Filtro Gaussiano", imagemOriginal)
                        idImagens += 1

                        'Gera o histograma da imagem para a as três intensidades
                        histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                        histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                        histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                        'Desativa a barra de progresso
                        progressoImagem.Visible = False
                        progressoImagem.Value = 0

                        'Reativa os botões da tela
                        ativaBotões()
                    Else
                        If (dimensoesMascara.SelectedItem.ToString.Equals("5x5")) Then

                            'Desativa os botões da tela para evitar escalonamento de atividades
                            desativaBotões()

                            Dim dadosImagem As Histograma = New Histograma()
                            'guarda o histograma no histórico
                            historicoHistograma.Add(dadosImagem)

                            frameFoto1.Image = frameFoto2.Image

                            'Ativa a barra de progresso
                            progressoImagem.Visible = True
                            'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                            progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 4) * (imagemOriginal.Height + 4))

                            'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                            imagemOriginal = filtroGaussiano5x5(imagemOriginal)
                            frameFoto2.Image = redimensiona(imagemOriginal)

                            'Adiciona a nova alteração ao histórico de imagens
                            tabelaHistorico.Rows.Add(CStr(idImagens), "5x5 Filtro Gaussiano", imagemOriginal)
                            idImagens += 1

                            'Gera o histograma da imagem para a as três intensidades
                            histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                            histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                            histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                            'Desativa a barra de progresso
                            progressoImagem.Visible = False
                            progressoImagem.Value = 0

                            'Reativa os botões da tela
                            ativaBotões()
                        Else
                            If (dimensoesMascara.SelectedItem.ToString.Equals("7x7")) Then

                                'Desativa os botões da tela para evitar escalonamento de atividades
                                desativaBotões()

                                Dim dadosImagem As Histograma = New Histograma()
                                'guarda o histograma no histórico
                                historicoHistograma.Add(dadosImagem)

                                frameFoto1.Image = frameFoto2.Image

                                'Ativa a barra de progresso
                                progressoImagem.Visible = True
                                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height + ((imagemOriginal.Width + 6) * (imagemOriginal.Height + 6))

                                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                                imagemOriginal = filtroGaussiano7x7(imagemOriginal)
                                frameFoto2.Image = redimensiona(imagemOriginal)

                                'Adiciona a nova alteração ao histórico de imagens
                                tabelaHistorico.Rows.Add(CStr(idImagens), "7x7 Filtro Gaussiano", imagemOriginal)
                                idImagens += 1

                                'Gera o histograma da imagem para a as três intensidades
                                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                                'Desativa a barra de progresso
                                progressoImagem.Visible = False
                                progressoImagem.Value = 0

                                'Reativa os botões da tela
                                ativaBotões()
                            End If
                        End If
                    End If
                End If
            Else
                MessageBox.Show("Selecione um tamanho para a máscara", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Else
            MessageBox.Show("Não há nenhuma imagem para ser alterada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    '-----------------------------------------------------------------------------------------------------------------------------------------------------
    '-----------------------------------BOTÕES DE FILTROS DE DILATAÇÃO / EROSÃO---------------------------------------------------------------------------

    'Botão de chamada para a função de filtro de Máximo
    Private Sub maximo_Click(sender As Object, e As EventArgs) Handles maximo.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            If Not (imagemAlterada) Then

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = filtroMaximo(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro de Máximo", imagemOriginal)
                idImagens += 1
                imagemAlterada = True

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            Else

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                frameFoto1.Image = frameFoto2.Image

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = filtroMaximo(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro de Máximo", imagemOriginal)
                idImagens += 1

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            End If
        Else
            MessageBox.Show("Não há nenhuma imagem para ser alterada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    'Botão de chamada para a função de filtro de Mínimo
    Private Sub minimo_Click(sender As Object, e As EventArgs) Handles minimo.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            If Not (imagemAlterada) Then

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = filtroMinimo(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro de Mínimo", imagemOriginal)
                idImagens += 1
                imagemAlterada = True

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            Else

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = imagemOriginal.Width * imagemOriginal.Height

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                frameFoto1.Image = frameFoto2.Image

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = filtroMinimo(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro de Mínimo", imagemOriginal)
                idImagens += 1

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            End If
        Else
            MessageBox.Show("Não há nenhuma imagem para ser alterada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    'Botão de chamada para a função de filtro de Abertura
    Private Sub abertura_Click(sender As Object, e As EventArgs) Handles abertura.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            If Not (imagemAlterada) Then

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = 2 * (imagemOriginal.Width * imagemOriginal.Height)

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = filtroAbertura(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro de Abertura", imagemOriginal)
                idImagens += 1
                imagemAlterada = True

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            Else

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = 2 * (imagemOriginal.Width * imagemOriginal.Height)

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                frameFoto1.Image = frameFoto2.Image

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = filtroAbertura(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro de Abertura", imagemOriginal)
                idImagens += 1

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            End If
        Else
            MessageBox.Show("Não há nenhuma imagem para ser alterada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    'Botão de chamada para a função de filtro de Fechamento
    Private Sub fechamento_Click(sender As Object, e As EventArgs) Handles fechamento.Click
        If (tabelaHistorico.Rows.Count > 1) Then
            If Not (imagemAlterada) Then

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = 2 * (imagemOriginal.Width * imagemOriginal.Height)

                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = filtroFechamento(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro de Fechamento", imagemOriginal)
                idImagens += 1
                imagemAlterada = True

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            Else

                'Desativa os botões da tela para evitar escalonamento de atividades
                desativaBotões()

                'Ativa a barra de progresso
                progressoImagem.Visible = True
                'Define o tamanho da barra de progresso de acordo com a quantidade de operações realizadas nas chamadas de funções
                progressoImagem.Maximum = 2 * (imagemOriginal.Width * imagemOriginal.Height)
                Dim dadosImagem As Histograma = New Histograma()
                'guarda o histograma no histórico
                historicoHistograma.Add(dadosImagem)

                frameFoto1.Image = frameFoto2.Image

                'Mantem a imaem original alterada e faz uma cópia redimensionada para caber no quado de amostra
                imagemOriginal = filtroFechamento(imagemOriginal)
                frameFoto2.Image = redimensiona(imagemOriginal)

                'Adiciona a nova alteração ao histórico de imagens
                tabelaHistorico.Rows.Add(CStr(idImagens), "Filtro de Fechamento", imagemOriginal)
                idImagens += 1

                'Gera o histograma da imagem para a as três intensidades
                histogramaImagemR.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeR)
                histogramaImagemG.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeG)
                histogramaImagemB.Series(0).Points.DataBindXY(intensidades, historicoHistograma(idImagens - 1).quantidadePorIntensidadeB)

                'Desativa a barra de progresso
                progressoImagem.Visible = False
                progressoImagem.Value = 0

                'Reativa os botões da tela
                ativaBotões()
            End If
        Else
            MessageBox.Show("Não há nenhuma imagem para ser alterada", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    '-----------------------------------------------------------------------------------------------------------------------------------------------------
    '-----------------------------------TRANSFORMAÇÕES DE INTENSIDADE-------------------------------------------------------------------------------------

    'Aplica transformação para tons de cinza na imagem
    Function tomDeCinza(image As Bitmap) As Bitmap
        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB, luminancia As Integer
        Dim altura As Integer = image.Height - 1
        Dim largura As Integer = image.Width - 1

        'para lidar com imagens coloridas, serão pegos os valores de R, G e B separadamente
        For i = 0 To largura
            For j = 0 To altura

                novaCorFinalR = image.GetPixel(i, j).R
                novaCorFinalG = image.GetPixel(i, j).G
                novaCorFinalB = image.GetPixel(i, j).B

                novaCorFinalR = Math.Round(novaCorFinalR * 0.3)
                novaCorFinalG = Math.Round(novaCorFinalG * 0.59)
                novaCorFinalB = Math.Round(novaCorFinalB * 0.11)

                luminancia = novaCorFinalR + novaCorFinalG + novaCorFinalB
                copiaTemp.SetPixel(i, j, Color.FromArgb(luminancia, luminancia, luminancia))

                'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                historicoHistograma(idImagens).quantidadePorIntensidadeR(luminancia + 1) += 1
                historicoHistograma(idImagens).quantidadePorIntensidadeG(luminancia + 1) += 1
                historicoHistograma(idImagens).quantidadePorIntensidadeB(luminancia + 1) += 1

                'Aumenta o progresso mostrado na barra de carregamento 
                progressoImagem.Value = progressoImagem.Value + 1

            Next
        Next
        Return copiaTemp

    End Function

    'Função para inverter a coloração da imagem
    Function inverteTons(image As Bitmap) As Bitmap
        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB As Integer
        Dim altura As Integer = image.Height - 1
        Dim largura As Integer = image.Width - 1

        'para lidar com imagens coloridas, serão pegos os valores de R, G e B separadamente
        For i = 0 To largura
            For j = 0 To altura

                novaCorFinalR = image.GetPixel(i, j).R
                novaCorFinalG = image.GetPixel(i, j).G
                novaCorFinalB = image.GetPixel(i, j).B

                novaCorFinalR = 255 - novaCorFinalR
                novaCorFinalG = 255 - novaCorFinalG
                novaCorFinalB = 255 - novaCorFinalB

                copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinalR, novaCorFinalG, novaCorFinalB))

                'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1

                'Aumenta o progresso mostrado na barra de carregamento 
                progressoImagem.Value = progressoImagem.Value + 1

            Next
        Next
        Return copiaTemp
    End Function

    'Função para binarizar a imagem
    Function binariza(image As Bitmap) As Bitmap
        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        Dim altura As Integer = image.Height - 1
        Dim largura As Integer = image.Width - 1

        'Passa a imagem para tons de cinza
        image = tomDeCinza(image)

        'Reseta histograma da imagem que foi alterado na chamada da função de tons de cinza
        historicoHistograma(idImagens) = New Histograma

        For i = 0 To largura
            For j = 0 To altura
                If (image.GetPixel(i, j).R <= 128) Then

                    copiaTemp.SetPixel(i, j, Color.FromArgb(0, 0, 0))

                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                    historicoHistograma(idImagens).quantidadePorIntensidadeR(1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeG(1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeB(1) += 1

                    progressoImagem.Value = progressoImagem.Value + 1
                Else
                    copiaTemp.SetPixel(i, j, Color.FromArgb(255, 255, 255))

                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                    historicoHistograma(idImagens).quantidadePorIntensidadeR(256) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeG(256) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeB(256) += 1

                    'Aumenta o progresso mostrado na barra de carregamento 
                    progressoImagem.Value = progressoImagem.Value + 1
                End If



            Next
        Next
        Return copiaTemp
    End Function

    '-----------------------------------------------------------------------------------------------------------------------------------------------------
    '-----------------------------------FILTROS PASSA ALTAS-----------------------------------------------------------------------------------------------

    'Aplica o filtro de sobel sob uma imagem que foi passada como parâmetro
    Function filtroSobel(image As Bitmap) As Bitmap
        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        'Dim cor As Color
        Dim novaCorLinha, novaCorColuna, novaCorFinal As Integer
        Dim d1, d2, d3, d4, dAux1, dAux2 As Integer
        Dim l1, l2, l3, l4 As Integer
        Dim altura As Integer = image.Height - 1
        Dim largura As Integer = image.Width - 1
        ' -d1  0  d2        d1  2*l3  d2
        '-2*l1 0 2*l2       0    0    0
        ' -d3  0  d4       -d3 -2*l4 -d4

        'Passa a imagem para tons de cinza
        image = tomDeCinza(image)

        'Reseta histograma da imagem que foi alterado na chamada da função de tons de cinza
        historicoHistograma(idImagens) = New Histograma

        For i = 0 To largura
            For j = 0 To altura
                If (i = 0 And j = 0) Then 'Quina superior esquerda da imagem

                    'Pega valores da linha inferior e da lateral direita da imagem para completar-se
                    dAux1 = image.GetPixel(0, altura).R
                    dAux2 = image.GetPixel(largura, 0).R
                    d1 = Math.Round((dAux1 + dAux2) / 2)
                    d2 = image.GetPixel(1, altura).R
                    l1 = image.GetPixel(largura, 1).R
                    l2 = image.GetPixel(1, 0).R
                    d3 = image.GetPixel(largura, 1).R
                    d4 = image.GetPixel(1, 1).R

                    novaCorLinha = -d1 + d2 - 2 * l1 + 2 * l2 - d3 + d4

                    l3 = dAux1
                    l4 = image.GetPixel(0, 1).R

                    novaCorColuna = d1 + 2 * l3 + d2 - d3 - 2 * l4 - d4

                    novaCorFinal = Math.Round(Math.Sqrt(Math.Pow(novaCorColuna, 2) + Math.Pow(novaCorLinha, 2)))
                Else

                    If (i = 0 And j = altura) Then 'Quina inferior esquerda
                        'Pega valores da linha superior e da lateral direita da imagem para completar-se
                        dAux1 = image.GetPixel(largura, j).R
                        dAux2 = image.GetPixel(1, 0).R
                        d1 = image.GetPixel(largura, j - 1).R
                        d2 = image.GetPixel(1, j - 1).R
                        l1 = image.GetPixel(largura, j).R
                        l2 = image.GetPixel(1, j).R
                        d3 = Math.Round((dAux1 + dAux2) / 2)
                        d4 = image.GetPixel(1, 0).R


                        novaCorLinha = -d1 + d2 - 2 * l1 + 2 * l2 - d3 + d4

                        l3 = image.GetPixel(i, j - 1).R
                        l4 = image.GetPixel(0, 0).R

                        novaCorColuna = d1 + 2 * l3 + d2 - d3 - 2 * l4 - d4

                        novaCorFinal = Math.Round(Math.Sqrt(Math.Pow(novaCorColuna, 2) + Math.Pow(novaCorLinha, 2)))
                    Else
                        If (i = largura And j = 0) Then 'Quina superior direita da imagem

                            'Pega linha inferior e lateral esquerda para completar-se
                            dAux1 = image.GetPixel(i, altura).R
                            dAux2 = image.GetPixel(0, 0).R
                            d1 = image.GetPixel(i - 1, altura).R
                            d2 = Math.Round((dAux1 + dAux2) / 2)
                            l1 = image.GetPixel(i, 0).R
                            l2 = image.GetPixel(0, 0).R
                            d3 = image.GetPixel(i - 1, 1).R
                            d4 = image.GetPixel(0, 1).R


                            novaCorLinha = -d1 + d2 - 2 * l1 + 2 * l2 - d3 + d4

                            l3 = image.GetPixel(i, altura).R
                            l4 = image.GetPixel(i, 1).R

                            novaCorColuna = d1 + 2 * l3 + d2 - d3 - 2 * l4 - d4

                            novaCorFinal = Math.Round(Math.Sqrt(Math.Pow(novaCorColuna, 2) + Math.Pow(novaCorLinha, 2)))

                        Else
                            If (i = 0) Then 'Lateral esquerda da imagem

                                'Pega valores da lateral direita da imagem para completar-se
                                d1 = image.GetPixel(largura, j - 1).R
                                d2 = image.GetPixel(1, j - 1).R
                                l1 = image.GetPixel(largura, j).R
                                l2 = image.GetPixel(1, j).R
                                d3 = image.GetPixel(largura, 1).R
                                d4 = image.GetPixel(largura, j + 1).R


                                novaCorLinha = -d1 + d2 - 2 * l1 + 2 * l2 - d3 + d4

                                l3 = image.GetPixel(i, j - 1).R
                                l4 = image.GetPixel(i, j + 1).R

                                novaCorColuna = d1 + 2 * l3 + d2 - d3 - 2 * l4 - d4

                                novaCorFinal = Math.Round(Math.Sqrt(Math.Pow(novaCorColuna, 2) + Math.Pow(novaCorLinha, 2)))
                            Else
                                If (j = 0) Then 'Linha superior da imagem

                                    'Pega valores da linha inferior da imagem para completar-se 
                                    d1 = image.GetPixel(i - 1, altura).R
                                    d2 = image.GetPixel(i + 1, altura).R
                                    l1 = image.GetPixel(i - 1, j).R
                                    l2 = image.GetPixel(i + 1, j).R
                                    d3 = image.GetPixel(i - 1, j + 1).R
                                    d4 = image.GetPixel(i + 1, j + 1).R


                                    novaCorLinha = -d1 + d2 - 2 * l1 + 2 * l2 - d3 + d4

                                    l3 = image.GetPixel(i, altura).R
                                    l4 = image.GetPixel(i, 1).R

                                    novaCorColuna = d1 + 2 * l3 + d2 - d3 - 2 * l4 - d4

                                    novaCorFinal = Math.Round(Math.Sqrt(Math.Pow(novaCorColuna, 2) + Math.Pow(novaCorLinha, 2)))
                                Else
                                    If (i = largura And j = altura) Then 'Quina inferior direita da imagem

                                        'Pega os valores da linha superior e da lateral esquerda da imagem

                                        dAux1 = image.GetPixel(i, 0).R
                                        dAux2 = image.GetPixel(0, j).R
                                        d1 = image.GetPixel(i - 1, j - 1).R
                                        d2 = image.GetPixel(0, j - 1).R
                                        l1 = image.GetPixel(i - 1, j).R
                                        l2 = image.GetPixel(0, j).R
                                        d3 = image.GetPixel(i - 1, 0).R
                                        d4 = Math.Round((dAux1 + dAux2) / 2)

                                        novaCorLinha = -d1 + d2 - 2 * l1 + 2 * l2 - d3 + d4

                                        l3 = image.GetPixel(i, j - 1).R
                                        l4 = image.GetPixel(i, 0).R

                                        novaCorColuna = d1 + 2 * l3 + d2 - d3 - 2 * l4 - d4

                                        novaCorFinal = Math.Round(Math.Sqrt(Math.Pow(novaCorColuna, 2) + Math.Pow(novaCorLinha, 2)))
                                    Else
                                        If (i = largura) Then 'Lateral direita da imagem

                                            'Pega valores da lateral esquerda da imagem para completar-se
                                            d1 = image.GetPixel(i - 1, j - 1).R
                                            d2 = image.GetPixel(0, j - 1).R
                                            l1 = image.GetPixel(i - 1, j).R
                                            l2 = image.GetPixel(0, j).R
                                            d3 = image.GetPixel(i - 1, j + 1).R
                                            d4 = image.GetPixel(0, j + 1).R


                                            novaCorLinha = -d1 + d2 - 2 * l1 + 2 * l2 - d3 + d4

                                            l3 = image.GetPixel(i, j - 1).R
                                            l4 = image.GetPixel(i, j + 1).R

                                            novaCorColuna = d1 + 2 * l3 + d2 - d3 - 2 * l4 - d4

                                            novaCorFinal = Math.Round(Math.Sqrt(Math.Pow(novaCorColuna, 2) + Math.Pow(novaCorLinha, 2)))
                                        Else
                                            If (j = altura) Then 'Linha inferior da imagem

                                                'Pega valores da linha superior da imagem para completar-se
                                                d1 = image.GetPixel(i - 1, j - 1).R
                                                d2 = image.GetPixel(i + 1, j - 1).R
                                                l1 = image.GetPixel(i - 1, j).R
                                                l2 = image.GetPixel(i + 1, j).R
                                                d3 = image.GetPixel(i - 1, 0).R
                                                d4 = image.GetPixel(i + 1, 0).R


                                                novaCorLinha = -d1 + d2 - 2 * l1 + 2 * l2 - d3 + d4

                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, 0).R

                                                novaCorColuna = d1 + 2 * l3 + d2 - d3 - 2 * l4 - d4

                                                novaCorFinal = Math.Round(Math.Sqrt(Math.Pow(novaCorColuna, 2) + Math.Pow(novaCorLinha, 2)))
                                            Else

                                                'Caso não esteja nas bordas da imagem
                                                d1 = image.GetPixel(i - 1, j - 1).R
                                                d2 = image.GetPixel(i + 1, j - 1).R
                                                l1 = image.GetPixel(i - 1, j).R
                                                l2 = image.GetPixel(i + 1, j).R
                                                d3 = image.GetPixel(i - 1, j + 1).R
                                                d4 = image.GetPixel(i + 1, j + 1).R


                                                novaCorLinha = -d1 + d2 - 2 * l1 + 2 * l2 - d3 + d4

                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, j + 1).R

                                                novaCorColuna = d1 + 2 * l3 + d2 - d3 - 2 * l4 - d4

                                                novaCorFinal = Math.Round(Math.Sqrt(Math.Pow(novaCorColuna, 2) + Math.Pow(novaCorLinha, 2)))
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
                'Insere o novo pixel na imagem temporária
                If (novaCorFinal >= 0 And novaCorFinal <= 255) Then
                    copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinal, novaCorFinal, novaCorFinal))

                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinal + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinal + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinal + 1) += 1
                Else
                    If (novaCorFinal > 255) Then
                        copiaTemp.SetPixel(i, j, Color.FromArgb(255, 255, 255))

                        'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                        historicoHistograma(idImagens).quantidadePorIntensidadeR(256) += 1
                        historicoHistograma(idImagens).quantidadePorIntensidadeG(256) += 1
                        historicoHistograma(idImagens).quantidadePorIntensidadeB(256) += 1
                    Else
                        copiaTemp.SetPixel(i, j, Color.FromArgb(0, 0, 0))

                        'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                        historicoHistograma(idImagens).quantidadePorIntensidadeR(1) += 1
                        historicoHistograma(idImagens).quantidadePorIntensidadeG(1) += 1
                        historicoHistograma(idImagens).quantidadePorIntensidadeB(1) += 1
                    End If
                End If

                'Aumenta o progresso mostrado na barra de carregamento 
                progressoImagem.Value = progressoImagem.Value + 1
            Next
        Next
        Return copiaTemp
    End Function

    'Função para aplicar o filtro Laplaciano na imagem
    Function filtroLaplaciano(image As Bitmap) As Bitmap

        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        'Dim cor As Color
        Dim novaCorFinalR As Integer
        Dim d1, d2, d3, d4, dAux1, dAux2 As Integer
        Dim l1, l2, l3, l4, centro As Integer
        Dim altura As Integer = image.Height - 1
        Dim largura As Integer = image.Width - 1
        'd1   l3   d2
        'l1 centro l2
        'd3   l4   d4

        'Passa a imagem para tons de cinza
        image = tomDeCinza(image)

        'Reseta histograma da imagem que foi alterado na chamada da função de tons de cinza
        historicoHistograma(idImagens) = New Histograma

        'Equação do filtro = Math.Pow(Math.Round(((l1 + l2 + l3 + l4 + d1 + d2 + d3 + d4) - (8 * centro)) / 8), 2)

        For i = 0 To largura
            For j = 0 To altura
                If (i = 0 And j = 0) Then 'Quina superior esquerda da imagem

                    'Pega valores de R da linha inferior e da lateral direita da imagem para completar-se
                    dAux1 = image.GetPixel(0, altura).R
                    dAux2 = image.GetPixel(largura, 0).R
                    d1 = Math.Round((dAux1 + dAux2) / 2)
                    d2 = image.GetPixel(1, altura).R
                    l1 = image.GetPixel(largura, 1).R
                    l2 = image.GetPixel(1, 0).R
                    d3 = image.GetPixel(largura, 1).R
                    d4 = image.GetPixel(1, 1).R
                    l3 = dAux1
                    l4 = image.GetPixel(0, 1).R
                    centro = image.GetPixel(i, j).R

                    'Distribui os valores em um vetor para encontrar o maior

                    novaCorFinalR = Math.Pow(Math.Round(((l1 + l2 + l3 + l4 + d1 + d2 + d3 + d4) - (8 * centro)) / 8), 2)
                Else

                    If (i = 0 And j = altura) Then 'Quina inferior esquerda

                        'Pega valores de R linha superior e da lateral direita da imagem para completar-se
                        dAux1 = image.GetPixel(largura, j).R
                        dAux2 = image.GetPixel(1, 0).R
                        d1 = image.GetPixel(largura, j - 1).R
                        d2 = image.GetPixel(1, j - 1).R
                        l1 = image.GetPixel(largura, j).R
                        l2 = image.GetPixel(1, j).R
                        d3 = Math.Round((dAux1 + dAux2) / 2)
                        d4 = image.GetPixel(1, 0).R
                        l3 = image.GetPixel(i, j - 1).R
                        l4 = image.GetPixel(0, 0).R
                        centro = image.GetPixel(i, j).R

                        'Distribui os valores em um vetor para encontrar o maior
                        novaCorFinalR = Math.Pow(Math.Round(((l1 + l2 + l3 + l4 + d1 + d2 + d3 + d4) - (8 * centro)) / 8), 2)
                    Else
                        If (i = largura And j = 0) Then 'Quina superior direita da imagem

                            'Pega valores de R da linha inferior e lateral esquerda para completar-se
                            dAux1 = image.GetPixel(i, altura).R
                            dAux2 = image.GetPixel(0, 0).R
                            d1 = image.GetPixel(i - 1, altura).R
                            d2 = Math.Round((dAux1 + dAux2) / 2)
                            l1 = image.GetPixel(i, 0).R
                            l2 = image.GetPixel(0, 0).R
                            d3 = image.GetPixel(i - 1, 1).R
                            d4 = image.GetPixel(0, 1).R
                            l3 = image.GetPixel(i, altura).R
                            l4 = image.GetPixel(i, 1).R
                            centro = image.GetPixel(i, j).R

                            'Distribui os valores em um vetor para encontrar o maior
                            novaCorFinalR = Math.Pow(Math.Round(((l1 + l2 + l3 + l4 + d1 + d2 + d3 + d4) - (8 * centro)) / 8), 2)
                        Else
                            If (i = 0) Then 'Lateral esquerda da imagem

                                'Pega valores de R da lateral direita da imagem para completar-se
                                d1 = image.GetPixel(largura, j - 1).R
                                d2 = image.GetPixel(1, j - 1).R
                                l1 = image.GetPixel(largura, j).R
                                l2 = image.GetPixel(1, j).R
                                d3 = image.GetPixel(largura, 1).R
                                d4 = image.GetPixel(largura, j + 1).R
                                l3 = image.GetPixel(i, j - 1).R
                                l4 = image.GetPixel(i, j + 1).R
                                centro = image.GetPixel(i, j).R

                                'Distribui os valores em um vetor para encontrar o maior
                                novaCorFinalR = Math.Pow(Math.Round(((l1 + l2 + l3 + l4 + d1 + d2 + d3 + d4) - (8 * centro)) / 8), 2)
                            Else
                                If (j = 0) Then 'Linha superior da imagem

                                    'Pega valores de R da linha inferior da imagem para completar-se 
                                    d1 = image.GetPixel(i - 1, altura).R
                                    d2 = image.GetPixel(i + 1, altura).R
                                    l1 = image.GetPixel(i - 1, j).R
                                    l2 = image.GetPixel(i + 1, j).R
                                    d3 = image.GetPixel(i - 1, j + 1).R
                                    d4 = image.GetPixel(i + 1, j + 1).R
                                    l3 = image.GetPixel(i, altura).R
                                    l4 = image.GetPixel(i, 1).R
                                    centro = image.GetPixel(i, j).R

                                    'Distribui os valores em um vetor para encontrar o maior
                                    novaCorFinalR = Math.Pow(Math.Round(((l1 + l2 + l3 + l4 + d1 + d2 + d3 + d4) - (8 * centro)) / 8), 2)
                                Else
                                    If (i = largura And j = altura) Then 'Quina inferior direita da imagem

                                        'Pega os valores de R da linha superior e da lateral esquerda da imagem
                                        dAux1 = image.GetPixel(i, 0).R
                                        dAux2 = image.GetPixel(0, j).R
                                        d1 = image.GetPixel(i - 1, j - 1).R
                                        d2 = image.GetPixel(0, j - 1).R
                                        l1 = image.GetPixel(i - 1, j).R
                                        l2 = image.GetPixel(0, j).R
                                        d3 = image.GetPixel(i - 1, 0).R
                                        d4 = Math.Round((dAux1 + dAux2) / 2)
                                        l3 = image.GetPixel(i, j - 1).R
                                        l4 = image.GetPixel(i, 0).R
                                        centro = image.GetPixel(i, j).R

                                        'Distribui os valores em um vetor para encontrar o maior
                                        novaCorFinalR = Math.Pow(Math.Round(((l1 + l2 + l3 + l4 + d1 + d2 + d3 + d4) - (8 * centro)) / 8), 2)
                                    Else
                                        If (i = largura) Then 'Lateral direita da imagem

                                            'Pega valores de R da lateral esquerda da imagem para completar-se
                                            d1 = image.GetPixel(i - 1, j - 1).R
                                            d2 = image.GetPixel(0, j - 1).R
                                            l1 = image.GetPixel(i - 1, j).R
                                            l2 = image.GetPixel(0, j).R
                                            d3 = image.GetPixel(i - 1, j + 1).R
                                            d4 = image.GetPixel(0, j + 1).R
                                            l3 = image.GetPixel(i, j - 1).R
                                            l4 = image.GetPixel(i, j + 1).R
                                            centro = image.GetPixel(i, j).R

                                            'Distribui os valores em um vetor para encontrar o maior
                                            novaCorFinalR = Math.Pow(Math.Round(((l1 + l2 + l3 + l4 + d1 + d2 + d3 + d4) - (8 * centro)) / 8), 2)
                                        Else
                                            If (j = altura) Then 'Linha inferior da imagem

                                                'Pega valores de R da linha superior da imagem para completar-se
                                                d1 = image.GetPixel(i - 1, j - 1).R
                                                d2 = image.GetPixel(i + 1, j - 1).R
                                                l1 = image.GetPixel(i - 1, j).R
                                                l2 = image.GetPixel(i + 1, j).R
                                                d3 = image.GetPixel(i - 1, 0).R
                                                d4 = image.GetPixel(i + 1, 0).R
                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, 0).R
                                                centro = image.GetPixel(i, j).R

                                                'Distribui os valores em um vetor para encontrar o maior
                                                novaCorFinalR = Math.Pow(Math.Round(((l1 + l2 + l3 + l4 + d1 + d2 + d3 + d4) - (8 * centro)) / 8), 2)
                                            Else

                                                'Caso não esteja nas bordas da imagem pega o valor de R
                                                d1 = image.GetPixel(i - 1, j - 1).R
                                                d2 = image.GetPixel(i + 1, j - 1).R
                                                l1 = image.GetPixel(i - 1, j).R
                                                l2 = image.GetPixel(i + 1, j).R
                                                d3 = image.GetPixel(i - 1, j + 1).R
                                                d4 = image.GetPixel(i + 1, j + 1).R
                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, j + 1).R
                                                centro = image.GetPixel(i, j).R

                                                'Distribui os valores em um vetor para encontrar o maior
                                                novaCorFinalR = Math.Pow(Math.Round(((l1 + l2 + l3 + l4 + d1 + d2 + d3 + d4) - (8 * centro)) / 8), 2)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
                'Insere o novo pixel na imagem temporária
                If (novaCorFinalR >= 0 And novaCorFinalR <= 255) Then
                    copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinalR, novaCorFinalR, novaCorFinalR))

                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalR + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalR + 1) += 1
                Else
                    If (novaCorFinalR > 255) Then
                        copiaTemp.SetPixel(i, j, Color.FromArgb(255, 255, 255))

                        'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                        historicoHistograma(idImagens).quantidadePorIntensidadeR(256) += 1
                        historicoHistograma(idImagens).quantidadePorIntensidadeG(256) += 1
                        historicoHistograma(idImagens).quantidadePorIntensidadeB(256) += 1
                    Else
                        copiaTemp.SetPixel(i, j, Color.FromArgb(0, 0, 0))

                        'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                        historicoHistograma(idImagens).quantidadePorIntensidadeR(1) += 1
                        historicoHistograma(idImagens).quantidadePorIntensidadeG(1) += 1
                        historicoHistograma(idImagens).quantidadePorIntensidadeB(1) += 1
                    End If
                End If

                'Aumenta o progresso mostrado na barra de carregamento 
                progressoImagem.Value = progressoImagem.Value + 1
            Next
        Next
        Return copiaTemp
    End Function

    ''Função para aplicar o filtro High-Boost na imagem
    Function filtroHighBoost(image As Bitmap, peso As Double) As Bitmap
        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        'Dim cor As Color
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB As Integer
        Dim d1, d2, d3, d4, dAux1, dAux2 As Integer
        Dim l1, l2, l3, l4, centro As Integer
        Dim altura As Integer = image.Height - 1
        Dim largura As Integer = image.Width - 1

        'd1   l3   d2
        'l1 centro l2
        'd3   l4   d4

        'Equação do filtro = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

        'para lidar com imágens coloridas, será aplicado o filtro nos valores de R, G e B separadamente
        For i = 0 To largura
            For j = 0 To altura
                If (i = 0 And j = 0) Then 'Quina superior esquerda da imagem

                    'Pega valores de R da linha inferior e da lateral direita da imagem para completar-se
                    dAux1 = image.GetPixel(0, altura).R
                    dAux2 = image.GetPixel(largura, 0).R
                    d1 = Math.Round((dAux1 + dAux2) / 2)
                    d2 = image.GetPixel(1, altura).R
                    l1 = image.GetPixel(largura, 1).R
                    l2 = image.GetPixel(1, 0).R
                    d3 = image.GetPixel(largura, 1).R
                    d4 = image.GetPixel(1, 1).R
                    l3 = dAux1
                    l4 = image.GetPixel(0, 1).R
                    centro = image.GetPixel(i, j).R

                    novaCorFinalR = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                    'Pega valores de G linha inferior e da lateral direita da imagem para completar-se
                    dAux1 = image.GetPixel(0, altura).G
                    dAux2 = image.GetPixel(largura, 0).G
                    d1 = Math.Round((dAux1 + dAux2) / 2)
                    d2 = image.GetPixel(1, altura).G
                    l1 = image.GetPixel(largura, 1).G
                    l2 = image.GetPixel(1, 0).G
                    d3 = image.GetPixel(largura, 1).G
                    d4 = image.GetPixel(1, 1).G
                    l3 = dAux1
                    l4 = image.GetPixel(0, 1).G
                    centro = image.GetPixel(i, j).G

                    novaCorFinalG = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                    'Pega valores de B linha inferior e da lateral direita da imagem para completar-se
                    dAux1 = image.GetPixel(0, altura).B
                    dAux2 = image.GetPixel(largura, 0).B
                    d1 = Math.Round((dAux1 + dAux2) / 2)
                    d2 = image.GetPixel(1, altura).B
                    l1 = image.GetPixel(largura, 1).B
                    l2 = image.GetPixel(1, 0).B
                    d3 = image.GetPixel(largura, 1).B
                    d4 = image.GetPixel(1, 1).B
                    l3 = dAux1
                    l4 = image.GetPixel(0, 1).B
                    centro = image.GetPixel(i, j).B

                    novaCorFinalB = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                Else
                    If (i = 0 And j = altura) Then 'Quina inferior esquerda
                        'Pega valores de R linha superior e da lateral direita da imagem para completar-se
                        dAux1 = image.GetPixel(largura, j).R
                        dAux2 = image.GetPixel(1, 0).R
                        d1 = image.GetPixel(largura, j - 1).R
                        d2 = image.GetPixel(1, j - 1).R
                        l1 = image.GetPixel(largura, j).R
                        l2 = image.GetPixel(1, j).R
                        d3 = Math.Round((dAux1 + dAux2) / 2)
                        d4 = image.GetPixel(1, 0).R
                        l3 = image.GetPixel(i, j - 1).R
                        l4 = image.GetPixel(0, 0).R
                        centro = image.GetPixel(i, j).R

                        novaCorFinalR = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                        'Pega valores de G linha superior e da lateral direita da imagem para completar-se
                        dAux1 = image.GetPixel(largura, j).G
                        dAux2 = image.GetPixel(1, 0).G
                        d1 = image.GetPixel(largura, j - 1).G
                        d2 = image.GetPixel(1, j - 1).G
                        l1 = image.GetPixel(largura, j).G
                        l2 = image.GetPixel(1, j).G
                        d3 = Math.Round((dAux1 + dAux2) / 2)
                        d4 = image.GetPixel(1, 0).G
                        l3 = image.GetPixel(i, j - 1).R
                        l4 = image.GetPixel(0, 0).R
                        centro = image.GetPixel(i, j).G

                        novaCorFinalG = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                        'Pega valores de B linha superior e da lateral direita da imagem para completar-se
                        dAux1 = image.GetPixel(largura, j).B
                        dAux2 = image.GetPixel(1, 0).B
                        d1 = image.GetPixel(largura, j - 1).B
                        d2 = image.GetPixel(1, j - 1).B
                        l1 = image.GetPixel(largura, j).B
                        l2 = image.GetPixel(1, j).B
                        d3 = Math.Round((dAux1 + dAux2) / 2)
                        d4 = image.GetPixel(1, 0).B
                        l3 = image.GetPixel(i, j - 1).R
                        l4 = image.GetPixel(0, 0).R
                        centro = image.GetPixel(i, j).B

                        novaCorFinalB = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)
                    Else
                        If (i = largura And j = 0) Then 'Quina superior direita da imagem
                            'Pega valores de R da linha inferior e lateral esquerda para completar-se
                            dAux1 = image.GetPixel(i, altura).R
                            dAux2 = image.GetPixel(0, 0).R
                            d1 = image.GetPixel(i - 1, altura).R
                            d2 = Math.Round((dAux1 + dAux2) / 2)
                            l1 = image.GetPixel(i, 0).R
                            l2 = image.GetPixel(0, 0).R
                            d3 = image.GetPixel(i - 1, 1).R
                            d4 = image.GetPixel(0, 1).R
                            l3 = image.GetPixel(i, altura).R
                            l4 = image.GetPixel(i, 1).R
                            centro = image.GetPixel(i, j).R

                            novaCorFinalR = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                            'Pega valores de G da linha inferior e lateral esquerda para completar-se
                            dAux1 = image.GetPixel(i, altura).G
                            dAux2 = image.GetPixel(0, 0).G
                            d1 = image.GetPixel(i - 1, altura).G
                            d2 = Math.Round((dAux1 + dAux2) / 2)
                            l1 = image.GetPixel(i, 0).G
                            l2 = image.GetPixel(0, 0).G
                            d3 = image.GetPixel(i - 1, 1).G
                            d4 = image.GetPixel(0, 1).G
                            l3 = image.GetPixel(i, altura).R
                            l4 = image.GetPixel(i, 1).R
                            centro = image.GetPixel(i, j).G

                            novaCorFinalG = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                            'Pega valores de B da linha inferior e lateral esquerda para completar-se
                            dAux1 = image.GetPixel(i, altura).B
                            dAux2 = image.GetPixel(0, 0).B
                            d1 = image.GetPixel(i - 1, altura).B
                            d2 = Math.Round((dAux1 + dAux2) / 2)
                            l1 = image.GetPixel(i, 0).B
                            l2 = image.GetPixel(0, 0).B
                            d3 = image.GetPixel(i - 1, 1).B
                            d4 = image.GetPixel(0, 1).B
                            l3 = image.GetPixel(i, altura).R
                            l4 = image.GetPixel(i, 1).R
                            centro = image.GetPixel(i, j).B

                            novaCorFinalB = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)
                        Else
                            If (i = 0) Then 'Lateral esquerda da imagem

                                'Pega valores de R da lateral direita da imagem para completar-se
                                d1 = image.GetPixel(largura, j - 1).R
                                d2 = image.GetPixel(1, j - 1).R
                                l1 = image.GetPixel(largura, j).R
                                l2 = image.GetPixel(1, j).R
                                d3 = image.GetPixel(largura, 1).R
                                d4 = image.GetPixel(largura, j + 1).R
                                l3 = image.GetPixel(i, j - 1).R
                                l4 = image.GetPixel(i, j + 1).R
                                centro = image.GetPixel(i, j).R

                                novaCorFinalR = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                                'Pega valores de G da lateral direita da imagem para completar-se
                                d1 = image.GetPixel(largura, j - 1).G
                                d2 = image.GetPixel(1, j - 1).G
                                l1 = image.GetPixel(largura, j).G
                                l2 = image.GetPixel(1, j).G
                                d3 = image.GetPixel(largura, 1).G
                                d4 = image.GetPixel(largura, j + 1).G
                                l3 = image.GetPixel(i, j - 1).R
                                l4 = image.GetPixel(i, j + 1).R
                                centro = image.GetPixel(i, j).G

                                novaCorFinalG = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                                'Pega valores de B da lateral direita da imagem para completar-se
                                d1 = image.GetPixel(largura, j - 1).B
                                d2 = image.GetPixel(1, j - 1).B
                                l1 = image.GetPixel(largura, j).B
                                l2 = image.GetPixel(1, j).B
                                d3 = image.GetPixel(largura, 1).B
                                d4 = image.GetPixel(largura, j + 1).B
                                l3 = image.GetPixel(i, j - 1).R
                                l4 = image.GetPixel(i, j + 1).R
                                centro = image.GetPixel(i, j).B

                                novaCorFinalB = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)
                            Else
                                If (j = 0) Then 'Linha superior da imagem

                                    'Pega valores de R da linha inferior da imagem para completar-se 
                                    d1 = image.GetPixel(i - 1, altura).R
                                    d2 = image.GetPixel(i + 1, altura).R
                                    l1 = image.GetPixel(i - 1, j).R
                                    l2 = image.GetPixel(i + 1, j).R
                                    d3 = image.GetPixel(i - 1, j + 1).R
                                    d4 = image.GetPixel(i + 1, j + 1).R
                                    l3 = image.GetPixel(i, altura).R
                                    l4 = image.GetPixel(i, 1).R
                                    centro = image.GetPixel(i, j).R

                                    novaCorFinalR = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                                    'Pega valores de G da linha inferior da imagem para completar-se 
                                    d1 = image.GetPixel(i - 1, altura).G
                                    d2 = image.GetPixel(i + 1, altura).G
                                    l1 = image.GetPixel(i - 1, j).G
                                    l2 = image.GetPixel(i + 1, j).G
                                    d3 = image.GetPixel(i - 1, j + 1).G
                                    d4 = image.GetPixel(i + 1, j + 1).G
                                    l3 = image.GetPixel(i, altura).R
                                    l4 = image.GetPixel(i, 1).R
                                    centro = image.GetPixel(i, j).G

                                    novaCorFinalG = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                                    'Pega valores de B da linha inferior da imagem para completar-se 
                                    d1 = image.GetPixel(i - 1, altura).B
                                    d2 = image.GetPixel(i + 1, altura).B
                                    l1 = image.GetPixel(i - 1, j).B
                                    l2 = image.GetPixel(i + 1, j).B
                                    d3 = image.GetPixel(i - 1, j + 1).B
                                    d4 = image.GetPixel(i + 1, j + 1).B
                                    l3 = image.GetPixel(i, altura).R
                                    l4 = image.GetPixel(i, 1).R
                                    centro = image.GetPixel(i, j).B

                                    novaCorFinalB = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)
                                Else
                                    If (i = largura And j = altura) Then 'Quina inferior direita da imagem

                                        'Pega os valores de R da linha superior e da lateral esquerda da imagem
                                        dAux1 = image.GetPixel(i, 0).R
                                        dAux2 = image.GetPixel(0, j).R
                                        d1 = image.GetPixel(i - 1, j - 1).R
                                        d2 = image.GetPixel(0, j - 1).R
                                        l1 = image.GetPixel(i - 1, j).R
                                        l2 = image.GetPixel(0, j).R
                                        d3 = image.GetPixel(i - 1, 0).R
                                        d4 = Math.Round((dAux1 + dAux2) / 2)
                                        l3 = image.GetPixel(i, j - 1).R
                                        l4 = image.GetPixel(i, 0).R
                                        centro = image.GetPixel(i, j).R

                                        novaCorFinalR = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                                        'Pega os valores de G da linha superior e da lateral esquerda da imagem
                                        dAux1 = image.GetPixel(i, 0).G
                                        dAux2 = image.GetPixel(0, j).G
                                        d1 = image.GetPixel(i - 1, j - 1).G
                                        d2 = image.GetPixel(0, j - 1).G
                                        l1 = image.GetPixel(i - 1, j).G
                                        l2 = image.GetPixel(0, j).G
                                        d3 = image.GetPixel(i - 1, 0).G
                                        d4 = Math.Round((dAux1 + dAux2) / 2)
                                        l3 = image.GetPixel(i, j - 1).R
                                        l4 = image.GetPixel(i, 0).R
                                        centro = image.GetPixel(i, j).G

                                        novaCorFinalG = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                                        'Pega os valores de B da linha superior e da lateral esquerda da imagem
                                        dAux1 = image.GetPixel(i, 0).B
                                        dAux2 = image.GetPixel(0, j).B
                                        d1 = image.GetPixel(i - 1, j - 1).B
                                        d2 = image.GetPixel(0, j - 1).B
                                        l1 = image.GetPixel(i - 1, j).B
                                        l2 = image.GetPixel(0, j).B
                                        d3 = image.GetPixel(i - 1, 0).B
                                        d4 = Math.Round((dAux1 + dAux2) / 2)
                                        l3 = image.GetPixel(i, j - 1).R
                                        l4 = image.GetPixel(i, 0).R
                                        centro = image.GetPixel(i, j).B

                                        novaCorFinalB = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)
                                    Else
                                        If (i = largura) Then 'Lateral direita da imagem

                                            'Pega valores de R da lateral esquerda da imagem para completar-se
                                            d1 = image.GetPixel(i - 1, j - 1).R
                                            d2 = image.GetPixel(0, j - 1).R
                                            l1 = image.GetPixel(i - 1, j).R
                                            l2 = image.GetPixel(0, j).R
                                            d3 = image.GetPixel(i - 1, j + 1).R
                                            d4 = image.GetPixel(0, j + 1).R
                                            l3 = image.GetPixel(i, j - 1).R
                                            l4 = image.GetPixel(i, j + 1).R
                                            centro = image.GetPixel(i, j).R

                                            novaCorFinalR = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                                            'Pega valores de G da lateral esquerda da imagem para completar-se
                                            d1 = image.GetPixel(i - 1, j - 1).G
                                            d2 = image.GetPixel(0, j - 1).G
                                            l1 = image.GetPixel(i - 1, j).G
                                            l2 = image.GetPixel(0, j).G
                                            d3 = image.GetPixel(i - 1, j + 1).G
                                            d4 = image.GetPixel(0, j + 1).G
                                            l3 = image.GetPixel(i, j - 1).R
                                            l4 = image.GetPixel(i, j + 1).R
                                            centro = image.GetPixel(i, j).G

                                            novaCorFinalG = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                                            'Pega valores de B da lateral esquerda da imagem para completar-se
                                            d1 = image.GetPixel(i - 1, j - 1).B
                                            d2 = image.GetPixel(0, j - 1).B
                                            l1 = image.GetPixel(i - 1, j).B
                                            l2 = image.GetPixel(0, j).B
                                            d3 = image.GetPixel(i - 1, j + 1).B
                                            d4 = image.GetPixel(0, j + 1).B
                                            l3 = image.GetPixel(i, j - 1).R
                                            l4 = image.GetPixel(i, j + 1).R
                                            centro = image.GetPixel(i, j).B

                                            novaCorFinalB = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)
                                        Else
                                            If (j = altura) Then 'Linha inferior da imagem

                                                'Pega valores de R da linha superior da imagem para completar-se
                                                d1 = image.GetPixel(i - 1, j - 1).R
                                                d2 = image.GetPixel(i + 1, j - 1).R
                                                l1 = image.GetPixel(i - 1, j).R
                                                l2 = image.GetPixel(i + 1, j).R
                                                d3 = image.GetPixel(i - 1, 0).R
                                                d4 = image.GetPixel(i + 1, 0).R
                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, 0).R
                                                centro = image.GetPixel(i, j).R

                                                novaCorFinalR = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                                                'Pega valores de G da linha superior da imagem para completar-se
                                                d1 = image.GetPixel(i - 1, j - 1).G
                                                d2 = image.GetPixel(i + 1, j - 1).G
                                                l1 = image.GetPixel(i - 1, j).G
                                                l2 = image.GetPixel(i + 1, j).G
                                                d3 = image.GetPixel(i - 1, 0).G
                                                d4 = image.GetPixel(i + 1, 0).G
                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, 0).R
                                                centro = image.GetPixel(i, j).G

                                                novaCorFinalG = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                                                'Pega valores de B da linha superior da imagem para completar-se
                                                d1 = image.GetPixel(i - 1, j - 1).B
                                                d2 = image.GetPixel(i + 1, j - 1).B
                                                l1 = image.GetPixel(i - 1, j).B
                                                l2 = image.GetPixel(i + 1, j).B
                                                d3 = image.GetPixel(i - 1, 0).B
                                                d4 = image.GetPixel(i + 1, 0).B
                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, 0).R
                                                centro = image.GetPixel(i, j).B

                                                novaCorFinalB = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)
                                            Else

                                                'Caso não esteja nas bordas da imagem pega o valor de R
                                                d1 = image.GetPixel(i - 1, j - 1).R
                                                d2 = image.GetPixel(i + 1, j - 1).R
                                                l1 = image.GetPixel(i - 1, j).R
                                                l2 = image.GetPixel(i + 1, j).R
                                                d3 = image.GetPixel(i - 1, j + 1).R
                                                d4 = image.GetPixel(i + 1, j + 1).R
                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, j + 1).R
                                                centro = image.GetPixel(i, j).R

                                                novaCorFinalR = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                                                'Caso não esteja nas bordas da imagem pega o valor de G
                                                d1 = image.GetPixel(i - 1, j - 1).G
                                                d2 = image.GetPixel(i + 1, j - 1).G
                                                l1 = image.GetPixel(i - 1, j).G
                                                l2 = image.GetPixel(i + 1, j).G
                                                d3 = image.GetPixel(i - 1, j + 1).G
                                                d4 = image.GetPixel(i + 1, j + 1).G
                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, j + 1).R
                                                centro = image.GetPixel(i, j).G

                                                novaCorFinalG = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)

                                                'Caso não esteja nas bordas da imagem pega o valor de B
                                                d1 = image.GetPixel(i - 1, j - 1).B
                                                d2 = image.GetPixel(i + 1, j - 1).B
                                                l1 = image.GetPixel(i - 1, j).B
                                                l2 = image.GetPixel(i + 1, j).B
                                                d3 = image.GetPixel(i - 1, j + 1).B
                                                d4 = image.GetPixel(i + 1, j + 1).B
                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, j + 1).R
                                                centro = image.GetPixel(i, j).B

                                                novaCorFinalB = Math.Round(((centro * ((9 * peso) - 1)) - (d1 + d2 + d3 + d4 + l1 + l2 + l3 + l4)) / 9)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                'Tratando tons fora do intervalo RGB 0,0,0 a 255,255,255
                If (novaCorFinalR >= 0 And novaCorFinalR <= 255) Then
                    If (novaCorFinalG >= 0 And novaCorFinalG <= 255) Then
                        If (novaCorFinalB >= 0 And novaCorFinalB <= 255) Then
                            copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinalR, novaCorFinalG, novaCorFinalB))

                            'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                            historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                            historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                            historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1
                        Else
                            If (novaCorFinalB >= 0) Then
                                copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinalR, novaCorFinalG, 255))

                                'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                                historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                                historicoHistograma(idImagens).quantidadePorIntensidadeB(256) += 1
                            Else
                                copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinalR, novaCorFinalG, 0))

                                'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                                historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                                historicoHistograma(idImagens).quantidadePorIntensidadeB(1) += 1
                            End If
                        End If
                    Else
                        If (novaCorFinalG >= 0) Then
                            If (novaCorFinalB >= 0 And novaCorFinalB <= 255) Then
                                copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinalR, 255, novaCorFinalB))

                                'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                                historicoHistograma(idImagens).quantidadePorIntensidadeG(256) += 1
                                historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1
                            Else
                                If (novaCorFinalB >= 0) Then
                                    copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinalR, 255, 255))

                                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeG(256) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeB(256) += 1
                                Else
                                    copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinalR, 255, 0))

                                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeG(256) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeB(1) += 1
                                End If
                            End If
                        Else
                            If (novaCorFinalB >= 0 And novaCorFinalB <= 255) Then
                                copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinalR, 0, novaCorFinalB))

                                'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                                historicoHistograma(idImagens).quantidadePorIntensidadeG(1) += 1
                                historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1
                            Else
                                If (novaCorFinalB >= 0) Then
                                    copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinalR, 0, 255))

                                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeG(1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeB(256) += 1
                                Else
                                    copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinalR, 0, 0))

                                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeG(1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeB(1) += 1
                                End If
                            End If
                        End If
                    End If

                Else
                    If (novaCorFinalR >= 0) Then
                        If (novaCorFinalG >= 0 And novaCorFinalG <= 255) Then
                            If (novaCorFinalB >= 0 And novaCorFinalB <= 255) Then
                                copiaTemp.SetPixel(i, j, Color.FromArgb(255, novaCorFinalG, novaCorFinalB))

                                'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                historicoHistograma(idImagens).quantidadePorIntensidadeR(256) += 1
                                historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                                historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1
                            Else
                                If (novaCorFinalB >= 0) Then
                                    copiaTemp.SetPixel(i, j, Color.FromArgb(255, novaCorFinalG, 255))

                                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                    historicoHistograma(idImagens).quantidadePorIntensidadeR(256) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeB(256) += 1
                                Else
                                    copiaTemp.SetPixel(i, j, Color.FromArgb(255, novaCorFinalG, 0))

                                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                    historicoHistograma(idImagens).quantidadePorIntensidadeR(256) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeB(1) += 1
                                End If
                            End If
                        Else
                            If (novaCorFinalG >= 0) Then
                                If (novaCorFinalB >= 0 And novaCorFinalB <= 255) Then
                                    copiaTemp.SetPixel(i, j, Color.FromArgb(255, 255, novaCorFinalB))

                                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                    historicoHistograma(idImagens).quantidadePorIntensidadeR(256) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeG(256) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1
                                Else
                                    If (novaCorFinalB >= 0) Then
                                        copiaTemp.SetPixel(i, j, Color.FromArgb(255, 255, 255))

                                        'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                        historicoHistograma(idImagens).quantidadePorIntensidadeR(256) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeG(256) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeB(256) += 1
                                    Else
                                        copiaTemp.SetPixel(i, j, Color.FromArgb(255, 255, 0))

                                        'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                        historicoHistograma(idImagens).quantidadePorIntensidadeR(256) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeG(256) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeB(1) += 1
                                    End If
                                End If
                            Else
                                If (novaCorFinalB >= 0 And novaCorFinalB <= 255) Then
                                    copiaTemp.SetPixel(i, j, Color.FromArgb(255, 0, novaCorFinalB))

                                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                    historicoHistograma(idImagens).quantidadePorIntensidadeR(256) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeG(1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1
                                Else
                                    If (novaCorFinalB >= 0) Then
                                        copiaTemp.SetPixel(i, j, Color.FromArgb(255, 0, 255))

                                        'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                        historicoHistograma(idImagens).quantidadePorIntensidadeR(256) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeG(1) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeB(256) += 1
                                    Else
                                        copiaTemp.SetPixel(i, j, Color.FromArgb(255, 0, 0))

                                        'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                        historicoHistograma(idImagens).quantidadePorIntensidadeR(256) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeG(1) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeB(1) += 1

                                    End If
                                End If
                            End If
                        End If
                    Else
                        If (novaCorFinalG >= 0 And novaCorFinalG <= 255) Then
                            If (novaCorFinalB >= 0 And novaCorFinalB <= 255) Then
                                copiaTemp.SetPixel(i, j, Color.FromArgb(0, novaCorFinalG, novaCorFinalB))

                                'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                historicoHistograma(idImagens).quantidadePorIntensidadeR(1) += 1
                                historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                                historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1
                            Else
                                If (novaCorFinalB >= 0) Then
                                    copiaTemp.SetPixel(i, j, Color.FromArgb(0, novaCorFinalG, 255))

                                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                    historicoHistograma(idImagens).quantidadePorIntensidadeR(1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeB(256) += 1
                                Else
                                    copiaTemp.SetPixel(i, j, Color.FromArgb(0, novaCorFinalG, 0))

                                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                    historicoHistograma(idImagens).quantidadePorIntensidadeR(1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeB(1) += 1
                                End If
                            End If
                        Else
                            If (novaCorFinalG >= 0) Then
                                If (novaCorFinalB >= 0 And novaCorFinalB <= 255) Then
                                    copiaTemp.SetPixel(i, j, Color.FromArgb(0, 255, novaCorFinalB))

                                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                    historicoHistograma(idImagens).quantidadePorIntensidadeR(1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeG(256) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1
                                Else
                                    If (novaCorFinalB >= 0) Then
                                        copiaTemp.SetPixel(i, j, Color.FromArgb(0, 255, 255))

                                        'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                        historicoHistograma(idImagens).quantidadePorIntensidadeR(1) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeG(256) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeB(256) += 1
                                    Else
                                        copiaTemp.SetPixel(i, j, Color.FromArgb(0, 255, 0))

                                        'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                        historicoHistograma(idImagens).quantidadePorIntensidadeR(1) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeG(256) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeB(1) += 1
                                    End If
                                End If
                            Else
                                If (novaCorFinalB >= 0 And novaCorFinalB <= 255) Then
                                    copiaTemp.SetPixel(i, j, Color.FromArgb(0, 0, novaCorFinalB))

                                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                    historicoHistograma(idImagens).quantidadePorIntensidadeR(1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeG(1) += 1
                                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1
                                Else
                                    If (novaCorFinalB >= 0) Then
                                        copiaTemp.SetPixel(i, j, Color.FromArgb(0, 0, 255))

                                        'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                        historicoHistograma(idImagens).quantidadePorIntensidadeR(1) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeG(1) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeB(256) += 1
                                    Else
                                        copiaTemp.SetPixel(i, j, Color.FromArgb(0, 0, 0))

                                        'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                                        historicoHistograma(idImagens).quantidadePorIntensidadeR(1) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeG(1) += 1
                                        historicoHistograma(idImagens).quantidadePorIntensidadeB(1) += 1
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                'Aumenta o progresso mostrado na barra de carregamento 
                progressoImagem.Value = progressoImagem.Value + 1
            Next
        Next
        Return copiaTemp
    End Function

    '-----------------------------------------------------------------------------------------------------------------------------------------------------
    '-----------------------------------FILTROS PASSA BAIXAS----------------------------------------------------------------------------------------------

    'Aplica o filtro de Média sob uma imagem que foi passada como parâmetro utilizando uma máscara 3x3
    Function filtroMedia3x3(image As Bitmap) As Bitmap

        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB As Integer
        Dim p1, p2, p3, p4, p5, p6, p7, p8, centro As Integer

        Dim imagemTemporaria As Bitmap = geraMatrizPara3x3(image)

        Dim altura As Integer = imagemTemporaria.Height - 1
        Dim largura As Integer = imagemTemporaria.Width - 1

        'p1     p2     p3
        'p4   centro   p5
        'p6     p7     p8

        'Equação do filtro = Math.Round((p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + centro) / 9)

        'para lidar com imágens coloridas, será aplicado o filtro nos valores de R, G e B separadamente
        For i = 0 To largura
            For j = 0 To altura
                If (i >= 1 And j >= 1 And i <= largura - 1 And j <= altura - 1) Then
                    'Caso não esteja nas bordas da imagem pega o valor de R
                    p1 = imagemTemporaria.GetPixel(i - 1, j - 1).R
                    p2 = imagemTemporaria.GetPixel(i, j - 1).R
                    p3 = imagemTemporaria.GetPixel(i + 1, j - 1).R
                    p4 = imagemTemporaria.GetPixel(i - 1, j).R
                    centro = imagemTemporaria.GetPixel(i, j).R
                    p5 = imagemTemporaria.GetPixel(i + 1, j).R
                    p6 = imagemTemporaria.GetPixel(i - 1, j + 1).R
                    p7 = imagemTemporaria.GetPixel(i, j + 1).R
                    p8 = imagemTemporaria.GetPixel(i + 1, j + 1).R

                    novaCorFinalR = Math.Round((p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + centro) / 9)

                    'Caso não esteja nas bordas da imagem pega o valor de G
                    p1 = imagemTemporaria.GetPixel(i - 1, j - 1).G
                    p2 = imagemTemporaria.GetPixel(i, j - 1).G
                    p3 = imagemTemporaria.GetPixel(i + 1, j - 1).G
                    p4 = imagemTemporaria.GetPixel(i - 1, j).G
                    centro = imagemTemporaria.GetPixel(i, j).G
                    p5 = imagemTemporaria.GetPixel(i + 1, j).G
                    p6 = imagemTemporaria.GetPixel(i - 1, j + 1).G
                    p7 = imagemTemporaria.GetPixel(i, j + 1).G
                    p8 = imagemTemporaria.GetPixel(i + 1, j + 1).G

                    novaCorFinalG = Math.Round((p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + centro) / 9)

                    'Caso não esteja nas bordas da imagem pega o valor de B
                    p1 = imagemTemporaria.GetPixel(i - 1, j - 1).B
                    p2 = imagemTemporaria.GetPixel(i, j - 1).B
                    p3 = imagemTemporaria.GetPixel(i + 1, j - 1).B
                    p4 = imagemTemporaria.GetPixel(i - 1, j).B
                    centro = imagemTemporaria.GetPixel(i, j).B
                    p5 = imagemTemporaria.GetPixel(i + 1, j).B
                    p6 = imagemTemporaria.GetPixel(i - 1, j + 1).B
                    p7 = imagemTemporaria.GetPixel(i, j + 1).B
                    p8 = imagemTemporaria.GetPixel(i + 1, j + 1).B

                    novaCorFinalB = Math.Round((p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + centro) / 9)

                    copiaTemp.SetPixel(i - 1, j - 1, Color.FromArgb(novaCorFinalR, novaCorFinalG, novaCorFinalB))

                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1

                    'Aumenta o progresso mostrado na barra de carregamento 
                    progressoImagem.Value = progressoImagem.Value + 1
                End If


            Next
        Next
        Return copiaTemp
    End Function

    'Aplica o filtro de Média sob uma imagem que foi passada como parâmetro utilizando uma máscara 5x5
    Function filtroMedia5x5(image As Bitmap) As Bitmap
        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        'Dim cor As Color
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB As Integer
        Dim p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20,
            p21, p22, p23, p24, centro As Integer

        Dim imagemTemporaria As Bitmap = geraMatrizPara5x5(image)

        Dim altura As Integer = imagemTemporaria.Height - 1
        Dim largura As Integer = imagemTemporaria.Width - 1

        'p1     p2     p3     p4      p5
        'p6     p7     p8     p9     p10
        'p11   p12   centro  p13     p14
        'p15   p16    p17    p18     p19
        'p20   p21    p22    p23     p24

        'Equação do filtro = Math.Round((p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11 + p12 + p13 + p14 + p15 + p16 +
        '                               p17 +p18 + p19 + p20 + p21 + p22 + p23 + p24 + centro) / 25)

        'para lidar com imágens coloridas, será aplicado o filtro nos valores de R, G e B separadamente
        For i = 0 To largura
            For j = 0 To altura
                If (i >= 2 And j >= 2 And i <= largura - 2 And j <= altura - 2) Then
                    'Caso não esteja nas bordas da imagem pega o valor de R
                    p1 = imagemTemporaria.GetPixel(i - 2, j - 2).R
                    p2 = imagemTemporaria.GetPixel(i - 1, j - 2).R
                    p3 = imagemTemporaria.GetPixel(i, j - 2).R
                    p4 = imagemTemporaria.GetPixel(i + 1, j - 2).R
                    p5 = imagemTemporaria.GetPixel(i - 2, j - 2).R
                    p6 = imagemTemporaria.GetPixel(i - 2, j - 1).R
                    p7 = imagemTemporaria.GetPixel(i - 1, j - 1).R
                    p8 = imagemTemporaria.GetPixel(i, j - 1).R
                    p9 = imagemTemporaria.GetPixel(i + 1, j - 1).R
                    p10 = imagemTemporaria.GetPixel(i + 2, j - 1).R
                    p11 = imagemTemporaria.GetPixel(i - 2, j).R
                    p12 = imagemTemporaria.GetPixel(i - 1, j).R
                    centro = imagemTemporaria.GetPixel(i, j).R
                    p13 = imagemTemporaria.GetPixel(i + 1, j).R
                    p14 = imagemTemporaria.GetPixel(i + 2, j).R
                    p15 = imagemTemporaria.GetPixel(i - 2, j + 1).R
                    p16 = imagemTemporaria.GetPixel(i - 1, j + 1).R
                    p17 = imagemTemporaria.GetPixel(i, j + 1).R
                    p18 = imagemTemporaria.GetPixel(i + 1, j + 1).R
                    p19 = imagemTemporaria.GetPixel(i + 2, j + 1).R
                    p20 = imagemTemporaria.GetPixel(i - 2, j + 2).R
                    p21 = imagemTemporaria.GetPixel(i - 1, j + 2).R
                    p22 = imagemTemporaria.GetPixel(i, j + 2).R
                    p23 = imagemTemporaria.GetPixel(i + 1, j + 2).R
                    p24 = imagemTemporaria.GetPixel(i + 2, j + 2).R

                    novaCorFinalR = Math.Round((p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11 + p12 + p13 + p14 + p15 + p16 +
                                               p17 + p18 + p19 + p20 + p21 + p22 + p23 + p24 + centro) / 25)

                    'Caso não esteja nas bordas da imagem pega o valor de G
                    p1 = imagemTemporaria.GetPixel(i - 2, j - 2).G
                    p2 = imagemTemporaria.GetPixel(i - 1, j - 2).G
                    p3 = imagemTemporaria.GetPixel(i, j - 2).G
                    p4 = imagemTemporaria.GetPixel(i + 1, j - 2).G
                    p5 = imagemTemporaria.GetPixel(i - 2, j - 2).G
                    p6 = imagemTemporaria.GetPixel(i - 2, j - 1).G
                    p7 = imagemTemporaria.GetPixel(i - 1, j - 1).G
                    p8 = imagemTemporaria.GetPixel(i, j - 1).G
                    p9 = imagemTemporaria.GetPixel(i + 1, j - 1).G
                    p10 = imagemTemporaria.GetPixel(i + 2, j - 1).G
                    p11 = imagemTemporaria.GetPixel(i - 2, j).G
                    p12 = imagemTemporaria.GetPixel(i - 1, j).G
                    centro = imagemTemporaria.GetPixel(i, j).G
                    p13 = imagemTemporaria.GetPixel(i + 1, j).G
                    p14 = imagemTemporaria.GetPixel(i + 2, j).G
                    p15 = imagemTemporaria.GetPixel(i - 2, j + 1).G
                    p16 = imagemTemporaria.GetPixel(i - 1, j + 1).G
                    p17 = imagemTemporaria.GetPixel(i, j + 1).G
                    p18 = imagemTemporaria.GetPixel(i + 1, j + 1).G
                    p19 = imagemTemporaria.GetPixel(i + 2, j + 1).G
                    p20 = imagemTemporaria.GetPixel(i - 2, j + 2).G
                    p21 = imagemTemporaria.GetPixel(i - 1, j + 2).G
                    p22 = imagemTemporaria.GetPixel(i, j + 2).G
                    p23 = imagemTemporaria.GetPixel(i + 1, j + 2).G
                    p24 = imagemTemporaria.GetPixel(i + 2, j + 2).G

                    novaCorFinalG = Math.Round((p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11 + p12 + p13 + p14 + p15 + p16 +
                                               p17 + p18 + p19 + p20 + p21 + p22 + p23 + p24 + centro) / 25)

                    'Caso não esteja nas bordas da imagem pega o valor de B
                    p1 = imagemTemporaria.GetPixel(i - 2, j - 2).B
                    p2 = imagemTemporaria.GetPixel(i - 1, j - 2).B
                    p3 = imagemTemporaria.GetPixel(i, j - 2).B
                    p4 = imagemTemporaria.GetPixel(i + 1, j - 2).B
                    p5 = imagemTemporaria.GetPixel(i - 2, j - 2).B
                    p6 = imagemTemporaria.GetPixel(i - 2, j - 1).B
                    p7 = imagemTemporaria.GetPixel(i - 1, j - 1).B
                    p8 = imagemTemporaria.GetPixel(i, j - 1).B
                    p9 = imagemTemporaria.GetPixel(i + 1, j - 1).B
                    p10 = imagemTemporaria.GetPixel(i + 2, j - 1).B
                    p11 = imagemTemporaria.GetPixel(i - 2, j).B
                    p12 = imagemTemporaria.GetPixel(i - 1, j).B
                    centro = imagemTemporaria.GetPixel(i, j).B
                    p13 = imagemTemporaria.GetPixel(i + 1, j).B
                    p14 = imagemTemporaria.GetPixel(i + 2, j).B
                    p15 = imagemTemporaria.GetPixel(i - 2, j + 1).B
                    p16 = imagemTemporaria.GetPixel(i - 1, j + 1).B
                    p17 = imagemTemporaria.GetPixel(i, j + 1).B
                    p18 = imagemTemporaria.GetPixel(i + 1, j + 1).B
                    p19 = imagemTemporaria.GetPixel(i + 2, j + 1).B
                    p20 = imagemTemporaria.GetPixel(i - 2, j + 2).B
                    p21 = imagemTemporaria.GetPixel(i - 1, j + 2).B
                    p22 = imagemTemporaria.GetPixel(i, j + 2).B
                    p23 = imagemTemporaria.GetPixel(i + 1, j + 2).B
                    p24 = imagemTemporaria.GetPixel(i + 2, j + 2).B

                    novaCorFinalB = Math.Round((p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11 + p12 + p13 + p14 + p15 + p16 +
                                               p17 + p18 + p19 + p20 + p21 + p22 + p23 + p24 + centro) / 25)

                    copiaTemp.SetPixel(i - 2, j - 2, Color.FromArgb(novaCorFinalR, novaCorFinalG, novaCorFinalB))

                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1

                    'Aumenta o progresso mostrado na barra de carregamento 
                    progressoImagem.Value = progressoImagem.Value + 1
                End If
            Next
        Next
        Return copiaTemp
    End Function

    'Aplica o filtro de Média sob uma imagem que foi passada como parâmetro utilizando uma máscara 7x7
    Function filtroMedia7x7(image As Bitmap) As Bitmap
        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        'Dim cor As Color
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB As Integer
        Dim p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24,
            p25, p26, p27, p28, p29, p30, p31, p32, p33, p34, p35, p36, p37, p38, p39, p40, p41, p42, p43, p44, p45, p46,
            p47, p48, pc As Integer

        Dim imagemTemporaria As Bitmap = geraMatrizPara7x7(image)

        Dim altura As Integer = imagemTemporaria.Height - 1
        Dim largura As Integer = imagemTemporaria.Width - 1

        ' p1  p2  p3  p4  p5  p6  p7
        ' p8  p9 p10 p11 p12 p13 p14
        'p15 p16 p17 p18 p19 p20 p21
        'p22 p23 p24  pc p25 p26 p27
        'p28 p29 p30 p31 p32 p33 p34
        'p35 p36 p37 p38 p39 p40 p41
        'p42 p43 p44 p45 p46 p47 p48

        'Equação do filtro = Math.Round((p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11 + p12 + p13 + p14 + p15 + p16 +
        '                               p17 +p18 + p19 + p20 + p21 + p22 + p23 + p24 + p25 + p26 + p27 + p28 + p29 + p30 + p31 +
        '                               p32 + p33 + p34 + p35 + p36 + p37 + p38 + p39 + p40 + p41 + p42 + p43 + p44 + p45 + p46 +
        '                               p47 + p48 + pc) / 49)

        'para lidar com imágens coloridas, será aplicado o filtro nos valores de R, G e B separadamente
        For i = 0 To largura
            For j = 0 To altura
                If (i >= 3 And j >= 3 And i <= largura - 3 And j <= altura - 3) Then
                    'Caso não esteja nas bordas da imagem pega o valor de R
                    p1 = imagemTemporaria.GetPixel(i - 3, j - 3).R
                    p2 = imagemTemporaria.GetPixel(i - 2, j - 3).R
                    p3 = imagemTemporaria.GetPixel(i - 1, j - 3).R
                    p4 = imagemTemporaria.GetPixel(i, j - 3).R
                    p5 = imagemTemporaria.GetPixel(i + 1, j - 3).R
                    p6 = imagemTemporaria.GetPixel(i + 2, j - 3).R
                    p7 = imagemTemporaria.GetPixel(i + 3, j - 3).R
                    p8 = imagemTemporaria.GetPixel(i - 3, j - 2).R
                    p9 = imagemTemporaria.GetPixel(i - 2, j - 2).R
                    p10 = imagemTemporaria.GetPixel(i - 1, j - 2).R
                    p11 = imagemTemporaria.GetPixel(i, j - 2).R
                    p12 = imagemTemporaria.GetPixel(i + 1, j - 2).R
                    p13 = imagemTemporaria.GetPixel(i + 2, j - 2).R
                    p14 = imagemTemporaria.GetPixel(i + 3, j - 2).R
                    p15 = imagemTemporaria.GetPixel(i - 3, j - 1).R
                    p16 = imagemTemporaria.GetPixel(i - 2, j - 1).R
                    p17 = imagemTemporaria.GetPixel(i - 1, j - 1).R
                    p18 = imagemTemporaria.GetPixel(i, j - 1).R
                    p19 = imagemTemporaria.GetPixel(i + 1, j - 1).R
                    p20 = imagemTemporaria.GetPixel(i + 2, j - 1).R
                    p21 = imagemTemporaria.GetPixel(i + 3, j - 1).R
                    p22 = imagemTemporaria.GetPixel(i - 3, j).R
                    p23 = imagemTemporaria.GetPixel(i - 2, j).R
                    p24 = imagemTemporaria.GetPixel(i - 1, j).R
                    pc = imagemTemporaria.GetPixel(i, j).R
                    p25 = imagemTemporaria.GetPixel(i + 1, j).R
                    p26 = imagemTemporaria.GetPixel(i + 2, j).R
                    p27 = imagemTemporaria.GetPixel(i + 3, j).R
                    p28 = imagemTemporaria.GetPixel(i - 3, j + 1).R
                    p29 = imagemTemporaria.GetPixel(i - 2, j + 1).R
                    p30 = imagemTemporaria.GetPixel(i - 1, j + 1).R
                    p31 = imagemTemporaria.GetPixel(i, j + 1).R
                    p32 = imagemTemporaria.GetPixel(i + 1, j + 1).R
                    p33 = imagemTemporaria.GetPixel(i + 2, j + 1).R
                    p34 = imagemTemporaria.GetPixel(i + 3, j + 1).R
                    p35 = imagemTemporaria.GetPixel(i - 3, j + 2).R
                    p36 = imagemTemporaria.GetPixel(i - 2, j + 2).R
                    p37 = imagemTemporaria.GetPixel(i - 1, j + 2).R
                    p38 = imagemTemporaria.GetPixel(i, j + 2).R
                    p39 = imagemTemporaria.GetPixel(i + 1, j + 2).R
                    p40 = imagemTemporaria.GetPixel(i + 2, j + 2).R
                    p41 = imagemTemporaria.GetPixel(i + 3, j + 2).R
                    p42 = imagemTemporaria.GetPixel(i - 3, j + 3).R
                    p43 = imagemTemporaria.GetPixel(i - 2, j + 3).R
                    p44 = imagemTemporaria.GetPixel(i - 1, j + 3).R
                    p45 = imagemTemporaria.GetPixel(i, j + 3).R
                    p46 = imagemTemporaria.GetPixel(i + 1, j + 3).R
                    p47 = imagemTemporaria.GetPixel(i + 2, j + 3).R
                    p48 = imagemTemporaria.GetPixel(i + 3, j + 3).R

                    novaCorFinalR = Math.Round((p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11 + p12 + p13 + p14 + p15 + p16 +
                                               p17 + p18 + p19 + p20 + p21 + p22 + p23 + p24 + p25 + p26 + p27 + p28 + p29 + p30 + p31 +
                                               p32 + p33 + p34 + p35 + p36 + p37 + p38 + p39 + p40 + p41 + p42 + p43 + p44 + p45 + p46 +
                                               p47 + p48 + pc) / 49)

                    'Caso não esteja nas bordas da imagem pega o valor de G
                    p1 = imagemTemporaria.GetPixel(i - 3, j - 3).G
                    p2 = imagemTemporaria.GetPixel(i - 2, j - 3).G
                    p3 = imagemTemporaria.GetPixel(i - 1, j - 3).G
                    p4 = imagemTemporaria.GetPixel(i, j - 3).G
                    p5 = imagemTemporaria.GetPixel(i + 1, j - 3).G
                    p6 = imagemTemporaria.GetPixel(i + 2, j - 3).G
                    p7 = imagemTemporaria.GetPixel(i + 3, j - 3).G
                    p8 = imagemTemporaria.GetPixel(i - 3, j - 2).G
                    p9 = imagemTemporaria.GetPixel(i - 2, j - 2).G
                    p10 = imagemTemporaria.GetPixel(i - 1, j - 2).G
                    p11 = imagemTemporaria.GetPixel(i, j - 2).G
                    p12 = imagemTemporaria.GetPixel(i + 1, j - 2).G
                    p13 = imagemTemporaria.GetPixel(i + 2, j - 2).G
                    p14 = imagemTemporaria.GetPixel(i + 3, j - 2).G
                    p15 = imagemTemporaria.GetPixel(i - 3, j - 1).G
                    p16 = imagemTemporaria.GetPixel(i - 2, j - 1).G
                    p17 = imagemTemporaria.GetPixel(i - 1, j - 1).G
                    p18 = imagemTemporaria.GetPixel(i, j - 1).G
                    p19 = imagemTemporaria.GetPixel(i + 1, j - 1).G
                    p20 = imagemTemporaria.GetPixel(i + 2, j - 1).G
                    p21 = imagemTemporaria.GetPixel(i + 3, j - 1).G
                    p22 = imagemTemporaria.GetPixel(i - 3, j).G
                    p23 = imagemTemporaria.GetPixel(i - 2, j).G
                    p24 = imagemTemporaria.GetPixel(i - 1, j).G
                    pc = imagemTemporaria.GetPixel(i, j).G
                    p25 = imagemTemporaria.GetPixel(i + 1, j).G
                    p26 = imagemTemporaria.GetPixel(i + 2, j).G
                    p27 = imagemTemporaria.GetPixel(i + 3, j).G
                    p28 = imagemTemporaria.GetPixel(i - 3, j + 1).G
                    p29 = imagemTemporaria.GetPixel(i - 2, j + 1).G
                    p30 = imagemTemporaria.GetPixel(i - 1, j + 1).G
                    p31 = imagemTemporaria.GetPixel(i, j + 1).G
                    p32 = imagemTemporaria.GetPixel(i + 1, j + 1).G
                    p33 = imagemTemporaria.GetPixel(i + 2, j + 1).G
                    p34 = imagemTemporaria.GetPixel(i + 3, j + 1).G
                    p35 = imagemTemporaria.GetPixel(i - 3, j + 2).G
                    p36 = imagemTemporaria.GetPixel(i - 2, j + 2).G
                    p37 = imagemTemporaria.GetPixel(i - 1, j + 2).G
                    p38 = imagemTemporaria.GetPixel(i, j + 2).G
                    p39 = imagemTemporaria.GetPixel(i + 1, j + 2).G
                    p40 = imagemTemporaria.GetPixel(i + 2, j + 2).G
                    p41 = imagemTemporaria.GetPixel(i + 3, j + 2).G
                    p42 = imagemTemporaria.GetPixel(i - 3, j + 3).G
                    p43 = imagemTemporaria.GetPixel(i - 2, j + 3).G
                    p44 = imagemTemporaria.GetPixel(i - 1, j + 3).G
                    p45 = imagemTemporaria.GetPixel(i, j + 3).G
                    p46 = imagemTemporaria.GetPixel(i + 1, j + 3).G
                    p47 = imagemTemporaria.GetPixel(i + 2, j + 3).G
                    p48 = imagemTemporaria.GetPixel(i + 3, j + 3).G

                    novaCorFinalG = Math.Round((p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11 + p12 + p13 + p14 + p15 + p16 +
                                               p17 + p18 + p19 + p20 + p21 + p22 + p23 + p24 + p25 + p26 + p27 + p28 + p29 + p30 + p31 +
                                               p32 + p33 + p34 + p35 + p36 + p37 + p38 + p39 + p40 + p41 + p42 + p43 + p44 + p45 + p46 +
                                               p47 + p48 + pc) / 49)

                    'Caso não esteja nas bordas da imagem pega o valor de B
                    p1 = imagemTemporaria.GetPixel(i - 3, j - 3).B
                    p2 = imagemTemporaria.GetPixel(i - 2, j - 3).B
                    p3 = imagemTemporaria.GetPixel(i - 1, j - 3).B
                    p4 = imagemTemporaria.GetPixel(i, j - 3).B
                    p5 = imagemTemporaria.GetPixel(i + 1, j - 3).B
                    p6 = imagemTemporaria.GetPixel(i + 2, j - 3).B
                    p7 = imagemTemporaria.GetPixel(i + 3, j - 3).B
                    p8 = imagemTemporaria.GetPixel(i - 3, j - 2).B
                    p9 = imagemTemporaria.GetPixel(i - 2, j - 2).B
                    p10 = imagemTemporaria.GetPixel(i - 1, j - 2).B
                    p11 = imagemTemporaria.GetPixel(i, j - 2).B
                    p12 = imagemTemporaria.GetPixel(i + 1, j - 2).B
                    p13 = imagemTemporaria.GetPixel(i + 2, j - 2).B
                    p14 = imagemTemporaria.GetPixel(i + 3, j - 2).B
                    p15 = imagemTemporaria.GetPixel(i - 3, j - 1).B
                    p16 = imagemTemporaria.GetPixel(i - 2, j - 1).B
                    p17 = imagemTemporaria.GetPixel(i - 1, j - 1).B
                    p18 = imagemTemporaria.GetPixel(i, j - 1).B
                    p19 = imagemTemporaria.GetPixel(i + 1, j - 1).B
                    p20 = imagemTemporaria.GetPixel(i + 2, j - 1).B
                    p21 = imagemTemporaria.GetPixel(i + 3, j - 1).B
                    p22 = imagemTemporaria.GetPixel(i - 3, j).B
                    p23 = imagemTemporaria.GetPixel(i - 2, j).B
                    p24 = imagemTemporaria.GetPixel(i - 1, j).B
                    pc = imagemTemporaria.GetPixel(i, j).B
                    p25 = imagemTemporaria.GetPixel(i + 1, j).B
                    p26 = imagemTemporaria.GetPixel(i + 2, j).B
                    p27 = imagemTemporaria.GetPixel(i + 3, j).B
                    p28 = imagemTemporaria.GetPixel(i - 3, j + 1).B
                    p29 = imagemTemporaria.GetPixel(i - 2, j + 1).B
                    p30 = imagemTemporaria.GetPixel(i - 1, j + 1).B
                    p31 = imagemTemporaria.GetPixel(i, j + 1).B
                    p32 = imagemTemporaria.GetPixel(i + 1, j + 1).B
                    p33 = imagemTemporaria.GetPixel(i + 2, j + 1).B
                    p34 = imagemTemporaria.GetPixel(i + 3, j + 1).B
                    p35 = imagemTemporaria.GetPixel(i - 3, j + 2).B
                    p36 = imagemTemporaria.GetPixel(i - 2, j + 2).B
                    p37 = imagemTemporaria.GetPixel(i - 1, j + 2).B
                    p38 = imagemTemporaria.GetPixel(i, j + 2).B
                    p39 = imagemTemporaria.GetPixel(i + 1, j + 2).B
                    p40 = imagemTemporaria.GetPixel(i + 2, j + 2).B
                    p41 = imagemTemporaria.GetPixel(i + 3, j + 2).B
                    p42 = imagemTemporaria.GetPixel(i - 3, j + 3).B
                    p43 = imagemTemporaria.GetPixel(i - 2, j + 3).B
                    p44 = imagemTemporaria.GetPixel(i - 1, j + 3).B
                    p45 = imagemTemporaria.GetPixel(i, j + 3).B
                    p46 = imagemTemporaria.GetPixel(i + 1, j + 3).B
                    p47 = imagemTemporaria.GetPixel(i + 2, j + 3).B
                    p48 = imagemTemporaria.GetPixel(i + 3, j + 3).B

                    novaCorFinalB = Math.Round((p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11 + p12 + p13 + p14 + p15 + p16 +
                                               p17 + p18 + p19 + p20 + p21 + p22 + p23 + p24 + p25 + p26 + p27 + p28 + p29 + p30 + p31 +
                                               p32 + p33 + p34 + p35 + p36 + p37 + p38 + p39 + p40 + p41 + p42 + p43 + p44 + p45 + p46 +
                                               p47 + p48 + pc) / 49)

                    copiaTemp.SetPixel(i - 3, j - 3, Color.FromArgb(novaCorFinalR, novaCorFinalG, novaCorFinalB))

                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1

                    'Aumenta o progresso mostrado na barra de carregamento 
                    progressoImagem.Value = progressoImagem.Value + 1
                End If
            Next
        Next
        Return copiaTemp
    End Function

    'Aplica o filtro de Mediana sob uma imagem que foi passada como parâmetro
    Function filtroMediana3x3(image As Bitmap) As Bitmap

        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB As Integer
        Dim p1, p2, p3, p4, p5, p6, p7, p8, centro As Integer

        Dim imagemTemporaria As Bitmap = geraMatrizPara3x3(image)

        Dim altura As Integer = imagemTemporaria.Height - 1
        Dim largura As Integer = imagemTemporaria.Width - 1

        Dim contador As Integer = 0

        Dim vetorCores() As Integer

        'p1     p2     p3
        'p4   centro   p5
        'p6     p7     p8

        'para lidar com imágens coloridas, será aplicado o filtro nos valores de R, G e B separadamente
        For i = 0 To largura
            For j = 0 To altura
                If (i >= 1 And j >= 1 And i <= largura - 1 And j <= altura - 1) Then
                    'Caso não esteja nas bordas da imagem pega o valor de R
                    p1 = imagemTemporaria.GetPixel(i - 1, j - 1).R
                    p2 = imagemTemporaria.GetPixel(i, j - 1).R
                    p3 = imagemTemporaria.GetPixel(i + 1, j - 1).R
                    p4 = imagemTemporaria.GetPixel(i - 1, j).R
                    centro = imagemTemporaria.GetPixel(i, j).R
                    p5 = imagemTemporaria.GetPixel(i + 1, j).R
                    p6 = imagemTemporaria.GetPixel(i - 1, j + 1).R
                    p7 = imagemTemporaria.GetPixel(i, j + 1).R
                    p8 = imagemTemporaria.GetPixel(i + 1, j + 1).R

                    vetorCores = {p1, p2, p3, p4, p5, p6, p7, p8, centro}
                    novaCorFinalR = valorMediano(vetorCores)

                    'Caso não esteja nas bordas da imagem pega o valor de G
                    p1 = imagemTemporaria.GetPixel(i - 1, j - 1).G
                    p2 = imagemTemporaria.GetPixel(i, j - 1).G
                    p3 = imagemTemporaria.GetPixel(i + 1, j - 1).G
                    p4 = imagemTemporaria.GetPixel(i - 1, j).G
                    centro = imagemTemporaria.GetPixel(i, j).G
                    p5 = imagemTemporaria.GetPixel(i + 1, j).G
                    p6 = imagemTemporaria.GetPixel(i - 1, j + 1).G
                    p7 = imagemTemporaria.GetPixel(i, j + 1).G
                    p8 = imagemTemporaria.GetPixel(i + 1, j + 1).G

                    vetorCores = {p1, p2, p3, p4, p5, p6, p7, p8, centro}
                    novaCorFinalG = valorMediano(vetorCores)

                    'Caso não esteja nas bordas da imagem pega o valor de B
                    p1 = imagemTemporaria.GetPixel(i - 1, j - 1).B
                    p2 = imagemTemporaria.GetPixel(i, j - 1).B
                    p3 = imagemTemporaria.GetPixel(i + 1, j - 1).B
                    p4 = imagemTemporaria.GetPixel(i - 1, j).B
                    centro = imagemTemporaria.GetPixel(i, j).B
                    p5 = imagemTemporaria.GetPixel(i + 1, j).B
                    p6 = imagemTemporaria.GetPixel(i - 1, j + 1).B
                    p7 = imagemTemporaria.GetPixel(i, j + 1).B
                    p8 = imagemTemporaria.GetPixel(i + 1, j + 1).B

                    vetorCores = {p1, p2, p3, p4, p5, p6, p7, p8, centro}
                    novaCorFinalB = valorMediano(vetorCores)

                    copiaTemp.SetPixel(i - 1, j - 1, Color.FromArgb(novaCorFinalR, novaCorFinalG, novaCorFinalB))

                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1

                    'Aumenta o progresso mostrado na barra de carregamento 
                    progressoImagem.Value = progressoImagem.Value + 1
                End If
            Next
        Next
        Return copiaTemp
    End Function

    'Aplica o filtro de Mediana sob uma imagem que foi passada como parâmetro
    Function filtroMediana5x5(image As Bitmap) As Bitmap

        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        'Dim cor As Color
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB As Integer
        Dim p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20,
            p21, p22, p23, p24, centro As Integer

        Dim imagemTemporaria As Bitmap = geraMatrizPara5x5(image)

        Dim altura As Integer = imagemTemporaria.Height - 1
        Dim largura As Integer = imagemTemporaria.Width - 1

        Dim vetorCores() As Integer

        'p1     p2     p3     p4      p5
        'p6     p7     p8     p9     p10
        'p11   p12   centro  p13     p14
        'p15   p16    p17    p18     p19
        'p20   p21    p22    p23     p24

        'para lidar com imágens coloridas, será aplicado o filtro nos valores de R, G e B separadamente
        For i = 0 To largura
            For j = 0 To altura
                If (i >= 2 And j >= 2 And i <= largura - 2 And j <= altura - 2) Then
                    'Caso não esteja nas bordas da imagem pega o valor de R
                    p1 = imagemTemporaria.GetPixel(i - 2, j - 2).R
                    p2 = imagemTemporaria.GetPixel(i - 1, j - 2).R
                    p3 = imagemTemporaria.GetPixel(i, j - 2).R
                    p4 = imagemTemporaria.GetPixel(i + 1, j - 2).R
                    p5 = imagemTemporaria.GetPixel(i - 2, j - 2).R
                    p6 = imagemTemporaria.GetPixel(i - 2, j - 1).R
                    p7 = imagemTemporaria.GetPixel(i - 1, j - 1).R
                    p8 = imagemTemporaria.GetPixel(i, j - 1).R
                    p9 = imagemTemporaria.GetPixel(i + 1, j - 1).R
                    p10 = imagemTemporaria.GetPixel(i + 2, j - 1).R
                    p11 = imagemTemporaria.GetPixel(i - 2, j).R
                    p12 = imagemTemporaria.GetPixel(i - 1, j).R
                    centro = imagemTemporaria.GetPixel(i, j).R
                    p13 = imagemTemporaria.GetPixel(i + 1, j).R
                    p14 = imagemTemporaria.GetPixel(i + 2, j).R
                    p15 = imagemTemporaria.GetPixel(i - 2, j + 1).R
                    p16 = imagemTemporaria.GetPixel(i - 1, j + 1).R
                    p17 = imagemTemporaria.GetPixel(i, j + 1).R
                    p18 = imagemTemporaria.GetPixel(i + 1, j + 1).R
                    p19 = imagemTemporaria.GetPixel(i + 2, j + 1).R
                    p20 = imagemTemporaria.GetPixel(i - 2, j + 2).R
                    p21 = imagemTemporaria.GetPixel(i - 1, j + 2).R
                    p22 = imagemTemporaria.GetPixel(i, j + 2).R
                    p23 = imagemTemporaria.GetPixel(i + 1, j + 2).R
                    p24 = imagemTemporaria.GetPixel(i + 2, j + 2).R

                    vetorCores = {p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16,
                                  p17, p18, p19, p20, p21, p22, p23, p24, centro}
                    novaCorFinalR = valorMediano(vetorCores)

                    'Caso não esteja nas bordas da imagem pega o valor de G
                    p1 = imagemTemporaria.GetPixel(i - 2, j - 2).G
                    p2 = imagemTemporaria.GetPixel(i - 1, j - 2).G
                    p3 = imagemTemporaria.GetPixel(i, j - 2).G
                    p4 = imagemTemporaria.GetPixel(i + 1, j - 2).G
                    p5 = imagemTemporaria.GetPixel(i - 2, j - 2).G
                    p6 = imagemTemporaria.GetPixel(i - 2, j - 1).G
                    p7 = imagemTemporaria.GetPixel(i - 1, j - 1).G
                    p8 = imagemTemporaria.GetPixel(i, j - 1).G
                    p9 = imagemTemporaria.GetPixel(i + 1, j - 1).G
                    p10 = imagemTemporaria.GetPixel(i + 2, j - 1).G
                    p11 = imagemTemporaria.GetPixel(i - 2, j).G
                    p12 = imagemTemporaria.GetPixel(i - 1, j).G
                    centro = imagemTemporaria.GetPixel(i, j).G
                    p13 = imagemTemporaria.GetPixel(i + 1, j).G
                    p14 = imagemTemporaria.GetPixel(i + 2, j).G
                    p15 = imagemTemporaria.GetPixel(i - 2, j + 1).G
                    p16 = imagemTemporaria.GetPixel(i - 1, j + 1).G
                    p17 = imagemTemporaria.GetPixel(i, j + 1).G
                    p18 = imagemTemporaria.GetPixel(i + 1, j + 1).G
                    p19 = imagemTemporaria.GetPixel(i + 2, j + 1).G
                    p20 = imagemTemporaria.GetPixel(i - 2, j + 2).G
                    p21 = imagemTemporaria.GetPixel(i - 1, j + 2).G
                    p22 = imagemTemporaria.GetPixel(i, j + 2).G
                    p23 = imagemTemporaria.GetPixel(i + 1, j + 2).G
                    p24 = imagemTemporaria.GetPixel(i + 2, j + 2).G

                    vetorCores = {p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16,
                                  p17, p18, p19, p20, p21, p22, p23, p24, centro}
                    novaCorFinalG = valorMediano(vetorCores)

                    'Caso não esteja nas bordas da imagem pega o valor de B
                    p1 = imagemTemporaria.GetPixel(i - 2, j - 2).B
                    p2 = imagemTemporaria.GetPixel(i - 1, j - 2).B
                    p3 = imagemTemporaria.GetPixel(i, j - 2).B
                    p4 = imagemTemporaria.GetPixel(i + 1, j - 2).B
                    p5 = imagemTemporaria.GetPixel(i - 2, j - 2).B
                    p6 = imagemTemporaria.GetPixel(i - 2, j - 1).B
                    p7 = imagemTemporaria.GetPixel(i - 1, j - 1).B
                    p8 = imagemTemporaria.GetPixel(i, j - 1).B
                    p9 = imagemTemporaria.GetPixel(i + 1, j - 1).B
                    p10 = imagemTemporaria.GetPixel(i + 2, j - 1).B
                    p11 = imagemTemporaria.GetPixel(i - 2, j).B
                    p12 = imagemTemporaria.GetPixel(i - 1, j).B
                    centro = imagemTemporaria.GetPixel(i, j).B
                    p13 = imagemTemporaria.GetPixel(i + 1, j).B
                    p14 = imagemTemporaria.GetPixel(i + 2, j).B
                    p15 = imagemTemporaria.GetPixel(i - 2, j + 1).B
                    p16 = imagemTemporaria.GetPixel(i - 1, j + 1).B
                    p17 = imagemTemporaria.GetPixel(i, j + 1).B
                    p18 = imagemTemporaria.GetPixel(i + 1, j + 1).B
                    p19 = imagemTemporaria.GetPixel(i + 2, j + 1).B
                    p20 = imagemTemporaria.GetPixel(i - 2, j + 2).B
                    p21 = imagemTemporaria.GetPixel(i - 1, j + 2).B
                    p22 = imagemTemporaria.GetPixel(i, j + 2).B
                    p23 = imagemTemporaria.GetPixel(i + 1, j + 2).B
                    p24 = imagemTemporaria.GetPixel(i + 2, j + 2).B

                    vetorCores = {p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16,
                                  p17, p18, p19, p20, p21, p22, p23, p24, centro}
                    novaCorFinalB = valorMediano(vetorCores)

                    copiaTemp.SetPixel(i - 2, j - 2, Color.FromArgb(novaCorFinalR, novaCorFinalG, novaCorFinalB))

                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1

                    'Aumenta o progresso mostrado na barra de carregamento 
                    progressoImagem.Value = progressoImagem.Value + 1
                End If
            Next
        Next

        Return copiaTemp
    End Function

    'Aplica o filtro de Mediana sob uma imagem que foi passada como parâmetro
    Function filtroMediana7x7(image As Bitmap) As Bitmap

        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        'Dim cor As Color
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB As Integer
        Dim p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24,
            p25, p26, p27, p28, p29, p30, p31, p32, p33, p34, p35, p36, p37, p38, p39, p40, p41, p42, p43, p44, p45, p46,
            p47, p48, pc As Integer

        Dim imagemTemporaria As Bitmap = geraMatrizPara7x7(image)

        Dim altura As Integer = imagemTemporaria.Height - 1
        Dim largura As Integer = imagemTemporaria.Width - 1

        Dim vetorCores() As Integer

        ' p1  p2  p3  p4  p5  p6  p7
        ' p8  p9 p10 p11 p12 p13 p14
        'p15 p16 p17 p18 p19 p20 p21
        'p22 p23 p24  pc p25 p26 p27
        'p28 p29 p30 p31 p32 p33 p34
        'p35 p36 p37 p38 p39 p40 p41
        'p42 p43 p44 p45 p46 p47 p48

        'Para lidar com imagens coloridas, será aplicado o filtro nos valores de R, G e B separadamente
        For i = 0 To largura
            For j = 0 To altura
                If (i >= 3 And j >= 3 And i <= largura - 3 And j <= altura - 3) Then
                    'Caso não esteja nas bordas da imagem pega o valor de R
                    p1 = imagemTemporaria.GetPixel(i - 3, j - 3).R
                    p2 = imagemTemporaria.GetPixel(i - 2, j - 3).R
                    p3 = imagemTemporaria.GetPixel(i - 1, j - 3).R
                    p4 = imagemTemporaria.GetPixel(i, j - 3).R
                    p5 = imagemTemporaria.GetPixel(i + 1, j - 3).R
                    p6 = imagemTemporaria.GetPixel(i + 2, j - 3).R
                    p7 = imagemTemporaria.GetPixel(i + 3, j - 3).R
                    p8 = imagemTemporaria.GetPixel(i - 3, j - 2).R
                    p9 = imagemTemporaria.GetPixel(i - 2, j - 2).R
                    p10 = imagemTemporaria.GetPixel(i - 1, j - 2).R
                    p11 = imagemTemporaria.GetPixel(i, j - 2).R
                    p12 = imagemTemporaria.GetPixel(i + 1, j - 2).R
                    p13 = imagemTemporaria.GetPixel(i + 2, j - 2).R
                    p14 = imagemTemporaria.GetPixel(i + 3, j - 2).R
                    p15 = imagemTemporaria.GetPixel(i - 3, j - 1).R
                    p16 = imagemTemporaria.GetPixel(i - 2, j - 1).R
                    p17 = imagemTemporaria.GetPixel(i - 1, j - 1).R
                    p18 = imagemTemporaria.GetPixel(i, j - 1).R
                    p19 = imagemTemporaria.GetPixel(i + 1, j - 1).R
                    p20 = imagemTemporaria.GetPixel(i + 2, j - 1).R
                    p21 = imagemTemporaria.GetPixel(i + 3, j - 1).R
                    p22 = imagemTemporaria.GetPixel(i - 3, j).R
                    p23 = imagemTemporaria.GetPixel(i - 2, j).R
                    p24 = imagemTemporaria.GetPixel(i - 1, j).R
                    pc = imagemTemporaria.GetPixel(i, j).R
                    p25 = imagemTemporaria.GetPixel(i + 1, j).R
                    p26 = imagemTemporaria.GetPixel(i + 2, j).R
                    p27 = imagemTemporaria.GetPixel(i + 3, j).R
                    p28 = imagemTemporaria.GetPixel(i - 3, j + 1).R
                    p29 = imagemTemporaria.GetPixel(i - 2, j + 1).R
                    p30 = imagemTemporaria.GetPixel(i - 1, j + 1).R
                    p31 = imagemTemporaria.GetPixel(i, j + 1).R
                    p32 = imagemTemporaria.GetPixel(i + 1, j + 1).R
                    p33 = imagemTemporaria.GetPixel(i + 2, j + 1).R
                    p34 = imagemTemporaria.GetPixel(i + 3, j + 1).R
                    p35 = imagemTemporaria.GetPixel(i - 3, j + 2).R
                    p36 = imagemTemporaria.GetPixel(i - 2, j + 2).R
                    p37 = imagemTemporaria.GetPixel(i - 1, j + 2).R
                    p38 = imagemTemporaria.GetPixel(i, j + 2).R
                    p39 = imagemTemporaria.GetPixel(i + 1, j + 2).R
                    p40 = imagemTemporaria.GetPixel(i + 2, j + 2).R
                    p41 = imagemTemporaria.GetPixel(i + 3, j + 2).R
                    p42 = imagemTemporaria.GetPixel(i - 3, j + 3).R
                    p43 = imagemTemporaria.GetPixel(i - 2, j + 3).R
                    p44 = imagemTemporaria.GetPixel(i - 1, j + 3).R
                    p45 = imagemTemporaria.GetPixel(i, j + 3).R
                    p46 = imagemTemporaria.GetPixel(i + 1, j + 3).R
                    p47 = imagemTemporaria.GetPixel(i + 2, j + 3).R
                    p48 = imagemTemporaria.GetPixel(i + 3, j + 3).R

                    vetorCores = {p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16,
                                  p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30, p31, p32,
                                  p33, p34, p35, p36, p37, p38, p39, p40, p41, p42, p43, p44, p45, p46, p47, p48, pc}
                    novaCorFinalR = valorMediano(vetorCores)

                    'Caso não esteja nas bordas da imagem pega o valor de G
                    p1 = imagemTemporaria.GetPixel(i - 3, j - 3).G
                    p2 = imagemTemporaria.GetPixel(i - 2, j - 3).G
                    p3 = imagemTemporaria.GetPixel(i - 1, j - 3).G
                    p4 = imagemTemporaria.GetPixel(i, j - 3).G
                    p5 = imagemTemporaria.GetPixel(i + 1, j - 3).G
                    p6 = imagemTemporaria.GetPixel(i + 2, j - 3).G
                    p7 = imagemTemporaria.GetPixel(i + 3, j - 3).G
                    p8 = imagemTemporaria.GetPixel(i - 3, j - 2).G
                    p9 = imagemTemporaria.GetPixel(i - 2, j - 2).G
                    p10 = imagemTemporaria.GetPixel(i - 1, j - 2).G
                    p11 = imagemTemporaria.GetPixel(i, j - 2).G
                    p12 = imagemTemporaria.GetPixel(i + 1, j - 2).G
                    p13 = imagemTemporaria.GetPixel(i + 2, j - 2).G
                    p14 = imagemTemporaria.GetPixel(i + 3, j - 2).G
                    p15 = imagemTemporaria.GetPixel(i - 3, j - 1).G
                    p16 = imagemTemporaria.GetPixel(i - 2, j - 1).G
                    p17 = imagemTemporaria.GetPixel(i - 1, j - 1).G
                    p18 = imagemTemporaria.GetPixel(i, j - 1).G
                    p19 = imagemTemporaria.GetPixel(i + 1, j - 1).G
                    p20 = imagemTemporaria.GetPixel(i + 2, j - 1).G
                    p21 = imagemTemporaria.GetPixel(i + 3, j - 1).G
                    p22 = imagemTemporaria.GetPixel(i - 3, j).G
                    p23 = imagemTemporaria.GetPixel(i - 2, j).G
                    p24 = imagemTemporaria.GetPixel(i - 1, j).G
                    pc = imagemTemporaria.GetPixel(i, j).G
                    p25 = imagemTemporaria.GetPixel(i + 1, j).G
                    p26 = imagemTemporaria.GetPixel(i + 2, j).G
                    p27 = imagemTemporaria.GetPixel(i + 3, j).G
                    p28 = imagemTemporaria.GetPixel(i - 3, j + 1).G
                    p29 = imagemTemporaria.GetPixel(i - 2, j + 1).G
                    p30 = imagemTemporaria.GetPixel(i - 1, j + 1).G
                    p31 = imagemTemporaria.GetPixel(i, j + 1).G
                    p32 = imagemTemporaria.GetPixel(i + 1, j + 1).G
                    p33 = imagemTemporaria.GetPixel(i + 2, j + 1).G
                    p34 = imagemTemporaria.GetPixel(i + 3, j + 1).G
                    p35 = imagemTemporaria.GetPixel(i - 3, j + 2).G
                    p36 = imagemTemporaria.GetPixel(i - 2, j + 2).G
                    p37 = imagemTemporaria.GetPixel(i - 1, j + 2).G
                    p38 = imagemTemporaria.GetPixel(i, j + 2).G
                    p39 = imagemTemporaria.GetPixel(i + 1, j + 2).G
                    p40 = imagemTemporaria.GetPixel(i + 2, j + 2).G
                    p41 = imagemTemporaria.GetPixel(i + 3, j + 2).G
                    p42 = imagemTemporaria.GetPixel(i - 3, j + 3).G
                    p43 = imagemTemporaria.GetPixel(i - 2, j + 3).G
                    p44 = imagemTemporaria.GetPixel(i - 1, j + 3).G
                    p45 = imagemTemporaria.GetPixel(i, j + 3).G
                    p46 = imagemTemporaria.GetPixel(i + 1, j + 3).G
                    p47 = imagemTemporaria.GetPixel(i + 2, j + 3).G
                    p48 = imagemTemporaria.GetPixel(i + 3, j + 3).G

                    vetorCores = {p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16,
                                  p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30, p31, p32,
                                  p33, p34, p35, p36, p37, p38, p39, p40, p41, p42, p43, p44, p45, p46, p47, p48, pc}
                    novaCorFinalG = valorMediano(vetorCores)

                    'Caso não esteja nas bordas da imagem pega o valor de B
                    p1 = imagemTemporaria.GetPixel(i - 3, j - 3).B
                    p2 = imagemTemporaria.GetPixel(i - 2, j - 3).B
                    p3 = imagemTemporaria.GetPixel(i - 1, j - 3).B
                    p4 = imagemTemporaria.GetPixel(i, j - 3).B
                    p5 = imagemTemporaria.GetPixel(i + 1, j - 3).B
                    p6 = imagemTemporaria.GetPixel(i + 2, j - 3).B
                    p7 = imagemTemporaria.GetPixel(i + 3, j - 3).B
                    p8 = imagemTemporaria.GetPixel(i - 3, j - 2).B
                    p9 = imagemTemporaria.GetPixel(i - 2, j - 2).B
                    p10 = imagemTemporaria.GetPixel(i - 1, j - 2).B
                    p11 = imagemTemporaria.GetPixel(i, j - 2).B
                    p12 = imagemTemporaria.GetPixel(i + 1, j - 2).B
                    p13 = imagemTemporaria.GetPixel(i + 2, j - 2).B
                    p14 = imagemTemporaria.GetPixel(i + 3, j - 2).B
                    p15 = imagemTemporaria.GetPixel(i - 3, j - 1).B
                    p16 = imagemTemporaria.GetPixel(i - 2, j - 1).B
                    p17 = imagemTemporaria.GetPixel(i - 1, j - 1).B
                    p18 = imagemTemporaria.GetPixel(i, j - 1).B
                    p19 = imagemTemporaria.GetPixel(i + 1, j - 1).B
                    p20 = imagemTemporaria.GetPixel(i + 2, j - 1).B
                    p21 = imagemTemporaria.GetPixel(i + 3, j - 1).B
                    p22 = imagemTemporaria.GetPixel(i - 3, j).B
                    p23 = imagemTemporaria.GetPixel(i - 2, j).B
                    p24 = imagemTemporaria.GetPixel(i - 1, j).B
                    pc = imagemTemporaria.GetPixel(i, j).B
                    p25 = imagemTemporaria.GetPixel(i + 1, j).B
                    p26 = imagemTemporaria.GetPixel(i + 2, j).B
                    p27 = imagemTemporaria.GetPixel(i + 3, j).B
                    p28 = imagemTemporaria.GetPixel(i - 3, j + 1).B
                    p29 = imagemTemporaria.GetPixel(i - 2, j + 1).B
                    p30 = imagemTemporaria.GetPixel(i - 1, j + 1).B
                    p31 = imagemTemporaria.GetPixel(i, j + 1).B
                    p32 = imagemTemporaria.GetPixel(i + 1, j + 1).B
                    p33 = imagemTemporaria.GetPixel(i + 2, j + 1).B
                    p34 = imagemTemporaria.GetPixel(i + 3, j + 1).B
                    p35 = imagemTemporaria.GetPixel(i - 3, j + 2).B
                    p36 = imagemTemporaria.GetPixel(i - 2, j + 2).B
                    p37 = imagemTemporaria.GetPixel(i - 1, j + 2).B
                    p38 = imagemTemporaria.GetPixel(i, j + 2).B
                    p39 = imagemTemporaria.GetPixel(i + 1, j + 2).B
                    p40 = imagemTemporaria.GetPixel(i + 2, j + 2).B
                    p41 = imagemTemporaria.GetPixel(i + 3, j + 2).B
                    p42 = imagemTemporaria.GetPixel(i - 3, j + 3).B
                    p43 = imagemTemporaria.GetPixel(i - 2, j + 3).B
                    p44 = imagemTemporaria.GetPixel(i - 1, j + 3).B
                    p45 = imagemTemporaria.GetPixel(i, j + 3).B
                    p46 = imagemTemporaria.GetPixel(i + 1, j + 3).B
                    p47 = imagemTemporaria.GetPixel(i + 2, j + 3).B
                    p48 = imagemTemporaria.GetPixel(i + 3, j + 3).B

                    vetorCores = {p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16,
                                  p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30, p31, p32,
                                  p33, p34, p35, p36, p37, p38, p39, p40, p41, p42, p43, p44, p45, p46, p47, p48, pc}
                    novaCorFinalB = valorMediano(vetorCores)

                    copiaTemp.SetPixel(i - 3, j - 3, Color.FromArgb(novaCorFinalR, novaCorFinalG, novaCorFinalB))

                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1

                    'Aumenta o progresso mostrado na barra de carregamento 
                    progressoImagem.Value = progressoImagem.Value + 1
                End If
            Next
        Next
        Return copiaTemp
    End Function

    'Aplica o filtro de Gaussiano sob uma imagem que foi passada como parâmetro
    Function filtroGaussiano3x3(image As Bitmap) As Bitmap

        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB As Integer
        Dim p1, p2, p3, p4, p5, p6, p7, p8, centro As Integer

        Dim imagemTemporaria As Bitmap = geraMatrizPara3x3(image)

        Dim altura As Integer = imagemTemporaria.Height - 1
        Dim largura As Integer = imagemTemporaria.Width - 1

        'p1     p2     p3
        'p4   centro   p5
        'p6     p7     p8

        'Equação do filtro =  Math.Round((p1 + 2 * p2 + p3 + 2 * p4 + 2 * p5 + p6 + 2 * p7 + p8 + 4 * centro) / 16)

        'para lidar com imágens coloridas, será aplicado o filtro nos valores de R, G e B separadamente
        For i = 0 To largura
            For j = 0 To altura
                If (i >= 1 And j >= 1 And i <= largura - 1 And j <= altura - 1) Then
                    'Caso não esteja nas bordas da imagem pega o valor de R
                    p1 = imagemTemporaria.GetPixel(i - 1, j - 1).R
                    p2 = imagemTemporaria.GetPixel(i, j - 1).R
                    p3 = imagemTemporaria.GetPixel(i + 1, j - 1).R
                    p4 = imagemTemporaria.GetPixel(i - 1, j).R
                    centro = imagemTemporaria.GetPixel(i, j).R
                    p5 = imagemTemporaria.GetPixel(i + 1, j).R
                    p6 = imagemTemporaria.GetPixel(i - 1, j + 1).R
                    p7 = imagemTemporaria.GetPixel(i, j + 1).R
                    p8 = imagemTemporaria.GetPixel(i + 1, j + 1).R

                    novaCorFinalR = Math.Round((p1 + 2 * p2 + p3 + 2 * p4 + 2 * p5 + p6 + 2 * p7 + p8 + 4 * centro) / 16)

                    'Caso não esteja nas bordas da imagem pega o valor de G
                    p1 = imagemTemporaria.GetPixel(i - 1, j - 1).G
                    p2 = imagemTemporaria.GetPixel(i, j - 1).G
                    p3 = imagemTemporaria.GetPixel(i + 1, j - 1).G
                    p4 = imagemTemporaria.GetPixel(i - 1, j).G
                    centro = imagemTemporaria.GetPixel(i, j).G
                    p5 = imagemTemporaria.GetPixel(i + 1, j).G
                    p6 = imagemTemporaria.GetPixel(i - 1, j + 1).G
                    p7 = imagemTemporaria.GetPixel(i, j + 1).G
                    p8 = imagemTemporaria.GetPixel(i + 1, j + 1).G

                    novaCorFinalG = Math.Round((p1 + 2 * p2 + p3 + 2 * p4 + 2 * p5 + p6 + 2 * p7 + p8 + 4 * centro) / 16)

                    'Caso não esteja nas bordas da imagem pega o valor de B
                    p1 = imagemTemporaria.GetPixel(i - 1, j - 1).B
                    p2 = imagemTemporaria.GetPixel(i, j - 1).B
                    p3 = imagemTemporaria.GetPixel(i + 1, j - 1).B
                    p4 = imagemTemporaria.GetPixel(i - 1, j).B
                    centro = imagemTemporaria.GetPixel(i, j).B
                    p5 = imagemTemporaria.GetPixel(i + 1, j).B
                    p6 = imagemTemporaria.GetPixel(i - 1, j + 1).B
                    p7 = imagemTemporaria.GetPixel(i, j + 1).B
                    p8 = imagemTemporaria.GetPixel(i + 1, j + 1).B

                    novaCorFinalB = Math.Round((p1 + 2 * p2 + p3 + 2 * p4 + 2 * p5 + p6 + 2 * p7 + p8 + 4 * centro) / 16)

                    copiaTemp.SetPixel(i - 1, j - 1, Color.FromArgb(novaCorFinalR, novaCorFinalG, novaCorFinalB))

                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1

                    'Aumenta o progresso mostrado na barra de carregamento 
                    progressoImagem.Value = progressoImagem.Value + 1
                End If
            Next
        Next

        Return copiaTemp
    End Function

    Function filtroGaussiano5x5(image As Bitmap) As Bitmap
        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        'Dim cor As Color
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB As Integer
        Dim p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20,
            p21, p22, p23, p24, centro As Integer

        Dim imagemTemporaria As Bitmap = geraMatrizPara5x5(image)

        Dim altura As Integer = imagemTemporaria.Height - 1
        Dim largura As Integer = imagemTemporaria.Width - 1

        'p1     p2     p3     p4      p5
        'p6     p7     p8     p9     p10
        'p11   p12   centro  p13     p14
        'p15   p16    p17    p18     p19
        'p20   p21    p22    p23     p24

        'Equação do filtro = Math.Round((p1 + 4 * p2 + 6 * p3 + 4 * p4 + p5 + 4 * p6 + 16 * p7 + 24 * p8 + 16 * p9 + 4 * p10 +
        '                               6 * p11 + 24 * p12 + 36 * centro + 24 * p13 + 6 * p14 + 4 * p15 + 16 * p16 + 24 * p17 +
        '                               16 * p18 + 4 * p19 + p20 + 4 * p21 + 6 * p22 + 4 * p23 + p24) / 256)

        'para lidar com imágens coloridas, será aplicado o filtro nos valores de R, G e B separadamente
        For i = 0 To largura
            For j = 0 To altura
                If (i >= 2 And j >= 2 And i <= largura - 2 And j <= altura - 2) Then
                    'Caso não esteja nas bordas da imagem pega o valor de R
                    p1 = imagemTemporaria.GetPixel(i - 2, j - 2).R
                    p2 = imagemTemporaria.GetPixel(i - 1, j - 2).R
                    p3 = imagemTemporaria.GetPixel(i, j - 2).R
                    p4 = imagemTemporaria.GetPixel(i + 1, j - 2).R
                    p5 = imagemTemporaria.GetPixel(i - 2, j - 2).R
                    p6 = imagemTemporaria.GetPixel(i - 2, j - 1).R
                    p7 = imagemTemporaria.GetPixel(i - 1, j - 1).R
                    p8 = imagemTemporaria.GetPixel(i, j - 1).R
                    p9 = imagemTemporaria.GetPixel(i + 1, j - 1).R
                    p10 = imagemTemporaria.GetPixel(i + 2, j - 1).R
                    p11 = imagemTemporaria.GetPixel(i - 2, j).R
                    p12 = imagemTemporaria.GetPixel(i - 1, j).R
                    centro = imagemTemporaria.GetPixel(i, j).R
                    p13 = imagemTemporaria.GetPixel(i + 1, j).R
                    p14 = imagemTemporaria.GetPixel(i + 2, j).R
                    p15 = imagemTemporaria.GetPixel(i - 2, j + 1).R
                    p16 = imagemTemporaria.GetPixel(i - 1, j + 1).R
                    p17 = imagemTemporaria.GetPixel(i, j + 1).R
                    p18 = imagemTemporaria.GetPixel(i + 1, j + 1).R
                    p19 = imagemTemporaria.GetPixel(i + 2, j + 1).R
                    p20 = imagemTemporaria.GetPixel(i - 2, j + 2).R
                    p21 = imagemTemporaria.GetPixel(i - 1, j + 2).R
                    p22 = imagemTemporaria.GetPixel(i, j + 2).R
                    p23 = imagemTemporaria.GetPixel(i + 1, j + 2).R
                    p24 = imagemTemporaria.GetPixel(i + 2, j + 2).R

                    novaCorFinalR = Math.Round((p1 + 4 * p2 + 6 * p3 + 4 * p4 + p5 + 4 * p6 + 16 * p7 + 24 * p8 + 16 * p9 + 4 * p10 +
                                               6 * p11 + 24 * p12 + 36 * centro + 24 * p13 + 6 * p14 + 4 * p15 + 16 * p16 + 24 * p17 +
                                               16 * p18 + 4 * p19 + p20 + 4 * p21 + 6 * p22 + 4 * p23 + p24) / 256)

                    'Caso não esteja nas bordas da imagem pega o valor de G
                    p1 = imagemTemporaria.GetPixel(i - 2, j - 2).G
                    p2 = imagemTemporaria.GetPixel(i - 1, j - 2).G
                    p3 = imagemTemporaria.GetPixel(i, j - 2).G
                    p4 = imagemTemporaria.GetPixel(i + 1, j - 2).G
                    p5 = imagemTemporaria.GetPixel(i - 2, j - 2).G
                    p6 = imagemTemporaria.GetPixel(i - 2, j - 1).G
                    p7 = imagemTemporaria.GetPixel(i - 1, j - 1).G
                    p8 = imagemTemporaria.GetPixel(i, j - 1).G
                    p9 = imagemTemporaria.GetPixel(i + 1, j - 1).G
                    p10 = imagemTemporaria.GetPixel(i + 2, j - 1).G
                    p11 = imagemTemporaria.GetPixel(i - 2, j).G
                    p12 = imagemTemporaria.GetPixel(i - 1, j).G
                    centro = imagemTemporaria.GetPixel(i, j).G
                    p13 = imagemTemporaria.GetPixel(i + 1, j).G
                    p14 = imagemTemporaria.GetPixel(i + 2, j).G
                    p15 = imagemTemporaria.GetPixel(i - 2, j + 1).G
                    p16 = imagemTemporaria.GetPixel(i - 1, j + 1).G
                    p17 = imagemTemporaria.GetPixel(i, j + 1).G
                    p18 = imagemTemporaria.GetPixel(i + 1, j + 1).G
                    p19 = imagemTemporaria.GetPixel(i + 2, j + 1).G
                    p20 = imagemTemporaria.GetPixel(i - 2, j + 2).G
                    p21 = imagemTemporaria.GetPixel(i - 1, j + 2).G
                    p22 = imagemTemporaria.GetPixel(i, j + 2).G
                    p23 = imagemTemporaria.GetPixel(i + 1, j + 2).G
                    p24 = imagemTemporaria.GetPixel(i + 2, j + 2).G

                    novaCorFinalG = Math.Round((p1 + 4 * p2 + 6 * p3 + 4 * p4 + p5 + 4 * p6 + 16 * p7 + 24 * p8 + 16 * p9 + 4 * p10 +
                                               6 * p11 + 24 * p12 + 36 * centro + 24 * p13 + 6 * p14 + 4 * p15 + 16 * p16 + 24 * p17 +
                                               16 * p18 + 4 * p19 + p20 + 4 * p21 + 6 * p22 + 4 * p23 + p24) / 256)

                    'Caso não esteja nas bordas da imagem pega o valor de B
                    p1 = imagemTemporaria.GetPixel(i - 2, j - 2).B
                    p2 = imagemTemporaria.GetPixel(i - 1, j - 2).B
                    p3 = imagemTemporaria.GetPixel(i, j - 2).B
                    p4 = imagemTemporaria.GetPixel(i + 1, j - 2).B
                    p5 = imagemTemporaria.GetPixel(i - 2, j - 2).B
                    p6 = imagemTemporaria.GetPixel(i - 2, j - 1).B
                    p7 = imagemTemporaria.GetPixel(i - 1, j - 1).B
                    p8 = imagemTemporaria.GetPixel(i, j - 1).B
                    p9 = imagemTemporaria.GetPixel(i + 1, j - 1).B
                    p10 = imagemTemporaria.GetPixel(i + 2, j - 1).B
                    p11 = imagemTemporaria.GetPixel(i - 2, j).B
                    p12 = imagemTemporaria.GetPixel(i - 1, j).B
                    centro = imagemTemporaria.GetPixel(i, j).B
                    p13 = imagemTemporaria.GetPixel(i + 1, j).B
                    p14 = imagemTemporaria.GetPixel(i + 2, j).B
                    p15 = imagemTemporaria.GetPixel(i - 2, j + 1).B
                    p16 = imagemTemporaria.GetPixel(i - 1, j + 1).B
                    p17 = imagemTemporaria.GetPixel(i, j + 1).B
                    p18 = imagemTemporaria.GetPixel(i + 1, j + 1).B
                    p19 = imagemTemporaria.GetPixel(i + 2, j + 1).B
                    p20 = imagemTemporaria.GetPixel(i - 2, j + 2).B
                    p21 = imagemTemporaria.GetPixel(i - 1, j + 2).B
                    p22 = imagemTemporaria.GetPixel(i, j + 2).B
                    p23 = imagemTemporaria.GetPixel(i + 1, j + 2).B
                    p24 = imagemTemporaria.GetPixel(i + 2, j + 2).B

                    novaCorFinalB = Math.Round((p1 + 4 * p2 + 6 * p3 + 4 * p4 + p5 + 4 * p6 + 16 * p7 + 24 * p8 + 16 * p9 + 4 * p10 +
                                               6 * p11 + 24 * p12 + 36 * centro + 24 * p13 + 6 * p14 + 4 * p15 + 16 * p16 + 24 * p17 +
                                               16 * p18 + 4 * p19 + p20 + 4 * p21 + 6 * p22 + 4 * p23 + p24) / 256)

                    copiaTemp.SetPixel(i - 2, j - 2, Color.FromArgb(novaCorFinalR, novaCorFinalG, novaCorFinalB))

                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1

                    'Aumenta o progresso mostrado na barra de carregamento 
                    progressoImagem.Value = progressoImagem.Value + 1
                End If
            Next
        Next
        Return copiaTemp
    End Function

    Function filtroGaussiano7x7(image As Bitmap) As Bitmap
        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        'Dim cor As Color
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB As Integer
        Dim p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24,
            p25, p26, p27, p28, p29, p30, p31, p32, p33, p34, p35, p36, p37, p38, p39, p40, p41, p42, p43, p44, p45, p46,
            p47, p48, pc As Integer

        Dim imagemTemporaria As Bitmap = geraMatrizPara7x7(image)

        Dim altura As Integer = imagemTemporaria.Height - 1
        Dim largura As Integer = imagemTemporaria.Width - 1

        ' p1  p2  p3  p4  p5  p6  p7
        ' p8  p9 p10 p11 p12 p13 p14
        'p15 p16 p17 p18 p19 p20 p21
        'p22 p23 p24  pc p25 p26 p27
        'p28 p29 p30 p31 p32 p33 p34
        'p35 p36 p37 p38 p39 p40 p41
        'p42 p43 p44 p45 p46 p47 p48

        'Equação do filtro = Math.Round((p1 + 6 * p2 + 15 * p3 + 20 * p4 + 15 * p5 + 6 * p6 + p7 +
        '                               6 * p8 + 36 * p9 + 90 * p10 + 120 * p11 + 90 * p12 + 36 * p13 + 6 * p14 +
        '                               15 * p15 + 90 * p16 + 225 * p17 + 300 * p18 + 225 * p19 + 90 * p20 + 15 * p21 +
        '                               20 * p22 + 120 * p23 + 300 * p24 + 400 * pc + 300 * p25 + 120 * p26 + 20 * p27 +
        '                               15 * p28 + 90 * p29 + 225 * p30 + 300 * p31 + 225 * p32 + 90 * p33 + 15 * p34 +
        '                               6 * p35 + 36 * p36 + 90 * p37 + 120 * p38 + 90 * p39 + 36 * p40 + 6 * p41 +
        '                               p42 +6 * p43 + 15 * p44 + 20 * p45 + 15 * p46 + 6 * p47 + p48) / 4096)

        'para lidar com imágens coloridas, será aplicado o filtro nos valores de R, G e B separadamente
        For i = 0 To largura
            For j = 0 To altura
                If (i >= 3 And j >= 3 And i <= largura - 3 And j <= altura - 3) Then
                    'Caso não esteja nas bordas da imagem pega o valor de R
                    p1 = imagemTemporaria.GetPixel(i - 3, j - 3).R
                    p2 = imagemTemporaria.GetPixel(i - 2, j - 3).R
                    p3 = imagemTemporaria.GetPixel(i - 1, j - 3).R
                    p4 = imagemTemporaria.GetPixel(i, j - 3).R
                    p5 = imagemTemporaria.GetPixel(i + 1, j - 3).R
                    p6 = imagemTemporaria.GetPixel(i + 2, j - 3).R
                    p7 = imagemTemporaria.GetPixel(i + 3, j - 3).R
                    p8 = imagemTemporaria.GetPixel(i - 3, j - 2).R
                    p9 = imagemTemporaria.GetPixel(i - 2, j - 2).R
                    p10 = imagemTemporaria.GetPixel(i - 1, j - 2).R
                    p11 = imagemTemporaria.GetPixel(i, j - 2).R
                    p12 = imagemTemporaria.GetPixel(i + 1, j - 2).R
                    p13 = imagemTemporaria.GetPixel(i + 2, j - 2).R
                    p14 = imagemTemporaria.GetPixel(i + 3, j - 2).R
                    p15 = imagemTemporaria.GetPixel(i - 3, j - 1).R
                    p16 = imagemTemporaria.GetPixel(i - 2, j - 1).R
                    p17 = imagemTemporaria.GetPixel(i - 1, j - 1).R
                    p18 = imagemTemporaria.GetPixel(i, j - 1).R
                    p19 = imagemTemporaria.GetPixel(i + 1, j - 1).R
                    p20 = imagemTemporaria.GetPixel(i + 2, j - 1).R
                    p21 = imagemTemporaria.GetPixel(i + 3, j - 1).R
                    p22 = imagemTemporaria.GetPixel(i - 3, j).R
                    p23 = imagemTemporaria.GetPixel(i - 2, j).R
                    p24 = imagemTemporaria.GetPixel(i - 1, j).R
                    pc = imagemTemporaria.GetPixel(i, j).R
                    p25 = imagemTemporaria.GetPixel(i + 1, j).R
                    p26 = imagemTemporaria.GetPixel(i + 2, j).R
                    p27 = imagemTemporaria.GetPixel(i + 3, j).R
                    p28 = imagemTemporaria.GetPixel(i - 3, j + 1).R
                    p29 = imagemTemporaria.GetPixel(i - 2, j + 1).R
                    p30 = imagemTemporaria.GetPixel(i - 1, j + 1).R
                    p31 = imagemTemporaria.GetPixel(i, j + 1).R
                    p32 = imagemTemporaria.GetPixel(i + 1, j + 1).R
                    p33 = imagemTemporaria.GetPixel(i + 2, j + 1).R
                    p34 = imagemTemporaria.GetPixel(i + 3, j + 1).R
                    p35 = imagemTemporaria.GetPixel(i - 3, j + 2).R
                    p36 = imagemTemporaria.GetPixel(i - 2, j + 2).R
                    p37 = imagemTemporaria.GetPixel(i - 1, j + 2).R
                    p38 = imagemTemporaria.GetPixel(i, j + 2).R
                    p39 = imagemTemporaria.GetPixel(i + 1, j + 2).R
                    p40 = imagemTemporaria.GetPixel(i + 2, j + 2).R
                    p41 = imagemTemporaria.GetPixel(i + 3, j + 2).R
                    p42 = imagemTemporaria.GetPixel(i - 3, j + 3).R
                    p43 = imagemTemporaria.GetPixel(i - 2, j + 3).R
                    p44 = imagemTemporaria.GetPixel(i - 1, j + 3).R
                    p45 = imagemTemporaria.GetPixel(i, j + 3).R
                    p46 = imagemTemporaria.GetPixel(i + 1, j + 3).R
                    p47 = imagemTemporaria.GetPixel(i + 2, j + 3).R
                    p48 = imagemTemporaria.GetPixel(i + 3, j + 3).R

                    novaCorFinalR = Math.Round((p1 + 6 * p2 + 15 * p3 + 20 * p4 + 15 * p5 + 6 * p6 + p7 +
                                               6 * p8 + 36 * p9 + 90 * p10 + 120 * p11 + 90 * p12 + 36 * p13 + 6 * p14 +
                                               15 * p15 + 90 * p16 + 225 * p17 + 300 * p18 + 225 * p19 + 90 * p20 + 15 * p21 +
                                               20 * p22 + 120 * p23 + 300 * p24 + 400 * pc + 300 * p25 + 120 * p26 + 20 * p27 +
                                               15 * p28 + 90 * p29 + 225 * p30 + 300 * p31 + 225 * p32 + 90 * p33 + 15 * p34 +
                                               6 * p35 + 36 * p36 + 90 * p37 + 120 * p38 + 90 * p39 + 36 * p40 + 6 * p41 +
                                               p42 + 6 * p43 + 15 * p44 + 20 * p45 + 15 * p46 + 6 * p47 + p48) / 4096)

                    'Caso não esteja nas bordas da imagem pega o valor de G
                    p1 = imagemTemporaria.GetPixel(i - 3, j - 3).G
                    p2 = imagemTemporaria.GetPixel(i - 2, j - 3).G
                    p3 = imagemTemporaria.GetPixel(i - 1, j - 3).G
                    p4 = imagemTemporaria.GetPixel(i, j - 3).G
                    p5 = imagemTemporaria.GetPixel(i + 1, j - 3).G
                    p6 = imagemTemporaria.GetPixel(i + 2, j - 3).G
                    p7 = imagemTemporaria.GetPixel(i + 3, j - 3).G
                    p8 = imagemTemporaria.GetPixel(i - 3, j - 2).G
                    p9 = imagemTemporaria.GetPixel(i - 2, j - 2).G
                    p10 = imagemTemporaria.GetPixel(i - 1, j - 2).G
                    p11 = imagemTemporaria.GetPixel(i, j - 2).G
                    p12 = imagemTemporaria.GetPixel(i + 1, j - 2).G
                    p13 = imagemTemporaria.GetPixel(i + 2, j - 2).G
                    p14 = imagemTemporaria.GetPixel(i + 3, j - 2).G
                    p15 = imagemTemporaria.GetPixel(i - 3, j - 1).G
                    p16 = imagemTemporaria.GetPixel(i - 2, j - 1).G
                    p17 = imagemTemporaria.GetPixel(i - 1, j - 1).G
                    p18 = imagemTemporaria.GetPixel(i, j - 1).G
                    p19 = imagemTemporaria.GetPixel(i + 1, j - 1).G
                    p20 = imagemTemporaria.GetPixel(i + 2, j - 1).G
                    p21 = imagemTemporaria.GetPixel(i + 3, j - 1).G
                    p22 = imagemTemporaria.GetPixel(i - 3, j).G
                    p23 = imagemTemporaria.GetPixel(i - 2, j).G
                    p24 = imagemTemporaria.GetPixel(i - 1, j).G
                    pc = imagemTemporaria.GetPixel(i, j).G
                    p25 = imagemTemporaria.GetPixel(i + 1, j).G
                    p26 = imagemTemporaria.GetPixel(i + 2, j).G
                    p27 = imagemTemporaria.GetPixel(i + 3, j).G
                    p28 = imagemTemporaria.GetPixel(i - 3, j + 1).G
                    p29 = imagemTemporaria.GetPixel(i - 2, j + 1).G
                    p30 = imagemTemporaria.GetPixel(i - 1, j + 1).G
                    p31 = imagemTemporaria.GetPixel(i, j + 1).G
                    p32 = imagemTemporaria.GetPixel(i + 1, j + 1).G
                    p33 = imagemTemporaria.GetPixel(i + 2, j + 1).G
                    p34 = imagemTemporaria.GetPixel(i + 3, j + 1).G
                    p35 = imagemTemporaria.GetPixel(i - 3, j + 2).G
                    p36 = imagemTemporaria.GetPixel(i - 2, j + 2).G
                    p37 = imagemTemporaria.GetPixel(i - 1, j + 2).G
                    p38 = imagemTemporaria.GetPixel(i, j + 2).G
                    p39 = imagemTemporaria.GetPixel(i + 1, j + 2).G
                    p40 = imagemTemporaria.GetPixel(i + 2, j + 2).G
                    p41 = imagemTemporaria.GetPixel(i + 3, j + 2).G
                    p42 = imagemTemporaria.GetPixel(i - 3, j + 3).G
                    p43 = imagemTemporaria.GetPixel(i - 2, j + 3).G
                    p44 = imagemTemporaria.GetPixel(i - 1, j + 3).G
                    p45 = imagemTemporaria.GetPixel(i, j + 3).G
                    p46 = imagemTemporaria.GetPixel(i + 1, j + 3).G
                    p47 = imagemTemporaria.GetPixel(i + 2, j + 3).G
                    p48 = imagemTemporaria.GetPixel(i + 3, j + 3).G

                    novaCorFinalG = Math.Round((p1 + 6 * p2 + 15 * p3 + 20 * p4 + 15 * p5 + 6 * p6 + p7 +
                                               6 * p8 + 36 * p9 + 90 * p10 + 120 * p11 + 90 * p12 + 36 * p13 + 6 * p14 +
                                               15 * p15 + 90 * p16 + 225 * p17 + 300 * p18 + 225 * p19 + 90 * p20 + 15 * p21 +
                                               20 * p22 + 120 * p23 + 300 * p24 + 400 * pc + 300 * p25 + 120 * p26 + 20 * p27 +
                                               15 * p28 + 90 * p29 + 225 * p30 + 300 * p31 + 225 * p32 + 90 * p33 + 15 * p34 +
                                               6 * p35 + 36 * p36 + 90 * p37 + 120 * p38 + 90 * p39 + 36 * p40 + 6 * p41 +
                                               p42 + 6 * p43 + 15 * p44 + 20 * p45 + 15 * p46 + 6 * p47 + p48) / 4096)

                    'Caso não esteja nas bordas da imagem pega o valor de B
                    p1 = imagemTemporaria.GetPixel(i - 3, j - 3).B
                    p2 = imagemTemporaria.GetPixel(i - 2, j - 3).B
                    p3 = imagemTemporaria.GetPixel(i - 1, j - 3).B
                    p4 = imagemTemporaria.GetPixel(i, j - 3).B
                    p5 = imagemTemporaria.GetPixel(i + 1, j - 3).B
                    p6 = imagemTemporaria.GetPixel(i + 2, j - 3).B
                    p7 = imagemTemporaria.GetPixel(i + 3, j - 3).B
                    p8 = imagemTemporaria.GetPixel(i - 3, j - 2).B
                    p9 = imagemTemporaria.GetPixel(i - 2, j - 2).B
                    p10 = imagemTemporaria.GetPixel(i - 1, j - 2).B
                    p11 = imagemTemporaria.GetPixel(i, j - 2).B
                    p12 = imagemTemporaria.GetPixel(i + 1, j - 2).B
                    p13 = imagemTemporaria.GetPixel(i + 2, j - 2).B
                    p14 = imagemTemporaria.GetPixel(i + 3, j - 2).B
                    p15 = imagemTemporaria.GetPixel(i - 3, j - 1).B
                    p16 = imagemTemporaria.GetPixel(i - 2, j - 1).B
                    p17 = imagemTemporaria.GetPixel(i - 1, j - 1).B
                    p18 = imagemTemporaria.GetPixel(i, j - 1).B
                    p19 = imagemTemporaria.GetPixel(i + 1, j - 1).B
                    p20 = imagemTemporaria.GetPixel(i + 2, j - 1).B
                    p21 = imagemTemporaria.GetPixel(i + 3, j - 1).B
                    p22 = imagemTemporaria.GetPixel(i - 3, j).B
                    p23 = imagemTemporaria.GetPixel(i - 2, j).B
                    p24 = imagemTemporaria.GetPixel(i - 1, j).B
                    pc = imagemTemporaria.GetPixel(i, j).B
                    p25 = imagemTemporaria.GetPixel(i + 1, j).B
                    p26 = imagemTemporaria.GetPixel(i + 2, j).B
                    p27 = imagemTemporaria.GetPixel(i + 3, j).B
                    p28 = imagemTemporaria.GetPixel(i - 3, j + 1).B
                    p29 = imagemTemporaria.GetPixel(i - 2, j + 1).B
                    p30 = imagemTemporaria.GetPixel(i - 1, j + 1).B
                    p31 = imagemTemporaria.GetPixel(i, j + 1).B
                    p32 = imagemTemporaria.GetPixel(i + 1, j + 1).B
                    p33 = imagemTemporaria.GetPixel(i + 2, j + 1).B
                    p34 = imagemTemporaria.GetPixel(i + 3, j + 1).B
                    p35 = imagemTemporaria.GetPixel(i - 3, j + 2).B
                    p36 = imagemTemporaria.GetPixel(i - 2, j + 2).B
                    p37 = imagemTemporaria.GetPixel(i - 1, j + 2).B
                    p38 = imagemTemporaria.GetPixel(i, j + 2).B
                    p39 = imagemTemporaria.GetPixel(i + 1, j + 2).B
                    p40 = imagemTemporaria.GetPixel(i + 2, j + 2).B
                    p41 = imagemTemporaria.GetPixel(i + 3, j + 2).B
                    p42 = imagemTemporaria.GetPixel(i - 3, j + 3).B
                    p43 = imagemTemporaria.GetPixel(i - 2, j + 3).B
                    p44 = imagemTemporaria.GetPixel(i - 1, j + 3).B
                    p45 = imagemTemporaria.GetPixel(i, j + 3).B
                    p46 = imagemTemporaria.GetPixel(i + 1, j + 3).B
                    p47 = imagemTemporaria.GetPixel(i + 2, j + 3).B
                    p48 = imagemTemporaria.GetPixel(i + 3, j + 3).B

                    novaCorFinalB = Math.Round((p1 + 6 * p2 + 15 * p3 + 20 * p4 + 15 * p5 + 6 * p6 + p7 +
                                               6 * p8 + 36 * p9 + 90 * p10 + 120 * p11 + 90 * p12 + 36 * p13 + 6 * p14 +
                                               15 * p15 + 90 * p16 + 225 * p17 + 300 * p18 + 225 * p19 + 90 * p20 + 15 * p21 +
                                               20 * p22 + 120 * p23 + 300 * p24 + 400 * pc + 300 * p25 + 120 * p26 + 20 * p27 +
                                               15 * p28 + 90 * p29 + 225 * p30 + 300 * p31 + 225 * p32 + 90 * p33 + 15 * p34 +
                                               6 * p35 + 36 * p36 + 90 * p37 + 120 * p38 + 90 * p39 + 36 * p40 + 6 * p41 +
                                               p42 + 6 * p43 + 15 * p44 + 20 * p45 + 15 * p46 + 6 * p47 + p48) / 4096)

                    copiaTemp.SetPixel(i - 3, j - 3, Color.FromArgb(novaCorFinalR, novaCorFinalG, novaCorFinalB))

                    'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                    historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                    historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1

                    'Aumenta o progresso mostrado na barra de carregamento 
                    progressoImagem.Value = progressoImagem.Value + 1
                End If
            Next
        Next
        Return copiaTemp
    End Function

    '-----------------------------------------------------------------------------------------------------------------------------------------------------
    '-----------------------------------FILTROS DE DILATAÇÃO / EROSÃO-------------------------------------------------------------------------------------------------

    'Aplica o filtro de Máximo sob uma imagem que foi passada como parâmetro
    Function filtroMaximo(image As Bitmap) As Bitmap
        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        'Dim cor As Color
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB As Integer
        Dim d1, d2, d3, d4, dAux1, dAux2 As Integer
        Dim l1, l2, l3, l4, centro As Integer
        Dim altura As Integer = image.Height - 1
        Dim largura As Integer = image.Width - 1
        'd1   l3   d2
        'l1 centro l2
        'd3   l4   d4

        For i = 0 To largura
            For j = 0 To altura
                If (i = 0 And j = 0) Then 'Quina superior esquerda da imagem

                    'Pega valores de R da linha inferior e da lateral direita da imagem para completar-se
                    dAux1 = image.GetPixel(0, altura).R
                    dAux2 = image.GetPixel(largura, 0).R
                    d1 = Math.Round((dAux1 + dAux2) / 2)
                    d2 = image.GetPixel(1, altura).R
                    l1 = image.GetPixel(largura, 1).R
                    l2 = image.GetPixel(1, 0).R
                    d3 = image.GetPixel(largura, 1).R
                    d4 = image.GetPixel(1, 1).R
                    l3 = dAux1
                    l4 = image.GetPixel(0, 1).R
                    centro = image.GetPixel(i, j).R

                    'Distribui os valores em um vetor para encontrar o maior
                    Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                    novaCorFinalR = valorMax(vetorCoresR)

                    'Pega valores de R da linha inferior e da lateral direita da imagem para completar-se
                    dAux1 = image.GetPixel(0, altura).G
                    dAux2 = image.GetPixel(largura, 0).G
                    d1 = Math.Round((dAux1 + dAux2) / 2)
                    d2 = image.GetPixel(1, altura).G
                    l1 = image.GetPixel(largura, 1).G
                    l2 = image.GetPixel(1, 0).G
                    d3 = image.GetPixel(largura, 1).G
                    d4 = image.GetPixel(1, 1).G
                    l3 = dAux1
                    l4 = image.GetPixel(0, 1).G
                    centro = image.GetPixel(i, j).G

                    'Distribui os valores em um vetor para encontrar o maior
                    Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                    novaCorFinalG = valorMax(vetorCoresG)

                    'Pega valores de R da linha inferior e da lateral direita da imagem para completar-se
                    dAux1 = image.GetPixel(0, altura).B
                    dAux2 = image.GetPixel(largura, 0).B
                    d1 = Math.Round((dAux1 + dAux2) / 2)
                    d2 = image.GetPixel(1, altura).B
                    l1 = image.GetPixel(largura, 1).B
                    l2 = image.GetPixel(1, 0).B
                    d3 = image.GetPixel(largura, 1).B
                    d4 = image.GetPixel(1, 1).B
                    l3 = dAux1
                    l4 = image.GetPixel(0, 1).B
                    centro = image.GetPixel(i, j).B

                    'Distribui os valores em um vetor para encontrar o maior
                    Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                    novaCorFinalB = valorMax(vetorCoresB)
                Else

                    If (i = 0 And j = altura) Then 'Quina inferior esquerda

                        'Pega valores de R linha superior e da lateral direita da imagem para completar-se
                        dAux1 = image.GetPixel(largura, j).R
                        dAux2 = image.GetPixel(1, 0).R
                        d1 = image.GetPixel(largura, j - 1).R
                        d2 = image.GetPixel(1, j - 1).R
                        l1 = image.GetPixel(largura, j).R
                        l2 = image.GetPixel(1, j).R
                        d3 = Math.Round((dAux1 + dAux2) / 2)
                        d4 = image.GetPixel(1, 0).R
                        l3 = image.GetPixel(i, j - 1).R
                        l4 = image.GetPixel(0, 0).R
                        centro = image.GetPixel(i, j).R

                        'Distribui os valores em um vetor para encontrar o maior
                        Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                        novaCorFinalR = valorMax(vetorCoresR)

                        'Pega valores de R linha superior e da lateral direita da imagem para completar-se
                        dAux1 = image.GetPixel(largura, j).G
                        dAux2 = image.GetPixel(1, 0).G
                        d1 = image.GetPixel(largura, j - 1).G
                        d2 = image.GetPixel(1, j - 1).G
                        l1 = image.GetPixel(largura, j).G
                        l2 = image.GetPixel(1, j).G
                        d3 = Math.Round((dAux1 + dAux2) / 2)
                        d4 = image.GetPixel(1, 0).G
                        l3 = image.GetPixel(i, j - 1).G
                        l4 = image.GetPixel(0, 0).G
                        centro = image.GetPixel(i, j).G

                        'Distribui os valores em um vetor para encontrar o maior
                        Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                        novaCorFinalG = valorMax(vetorCoresG)

                        'Pega valores de R linha superior e da lateral direita da imagem para completar-se
                        dAux1 = image.GetPixel(largura, j).B
                        dAux2 = image.GetPixel(1, 0).B
                        d1 = image.GetPixel(largura, j - 1).B
                        d2 = image.GetPixel(1, j - 1).B
                        l1 = image.GetPixel(largura, j).B
                        l2 = image.GetPixel(1, j).B
                        d3 = Math.Round((dAux1 + dAux2) / 2)
                        d4 = image.GetPixel(1, 0).B
                        l3 = image.GetPixel(i, j - 1).B
                        l4 = image.GetPixel(0, 0).B
                        centro = image.GetPixel(i, j).B

                        'Distribui os valores em um vetor para encontrar o maior
                        Dim vetorCores() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                        novaCorFinalB = valorMax(vetorCores)
                    Else
                        If (i = largura And j = 0) Then 'Quina superior direita da imagem

                            'Pega valores de R da linha inferior e lateral esquerda para completar-se
                            dAux1 = image.GetPixel(i, altura).R
                            dAux2 = image.GetPixel(0, 0).R
                            d1 = image.GetPixel(i - 1, altura).R
                            d2 = Math.Round((dAux1 + dAux2) / 2)
                            l1 = image.GetPixel(i, 0).R
                            l2 = image.GetPixel(0, 0).R
                            d3 = image.GetPixel(i - 1, 1).R
                            d4 = image.GetPixel(0, 1).R
                            l3 = image.GetPixel(i, altura).R
                            l4 = image.GetPixel(i, 1).R
                            centro = image.GetPixel(i, j).R

                            'Distribui os valores em um vetor para encontrar o maior
                            Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                            novaCorFinalR = valorMax(vetorCoresR)

                            'Pega valores de R da linha inferior e lateral esquerda para completar-se
                            dAux1 = image.GetPixel(i, altura).G
                            dAux2 = image.GetPixel(0, 0).G
                            d1 = image.GetPixel(i - 1, altura).G
                            d2 = Math.Round((dAux1 + dAux2) / 2)
                            l1 = image.GetPixel(i, 0).G
                            l2 = image.GetPixel(0, 0).G
                            d3 = image.GetPixel(i - 1, 1).G
                            d4 = image.GetPixel(0, 1).G
                            l3 = image.GetPixel(i, altura).G
                            l4 = image.GetPixel(i, 1).G
                            centro = image.GetPixel(i, j).G

                            'Distribui os valores em um vetor para encontrar o maior
                            Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                            novaCorFinalG = valorMax(vetorCoresG)

                            'Pega valores de R da linha inferior e lateral esquerda para completar-se
                            dAux1 = image.GetPixel(i, altura).B
                            dAux2 = image.GetPixel(0, 0).B
                            d1 = image.GetPixel(i - 1, altura).B
                            d2 = Math.Round((dAux1 + dAux2) / 2)
                            l1 = image.GetPixel(i, 0).B
                            l2 = image.GetPixel(0, 0).B
                            d3 = image.GetPixel(i - 1, 1).B
                            d4 = image.GetPixel(0, 1).B
                            l3 = image.GetPixel(i, altura).B
                            l4 = image.GetPixel(i, 1).B
                            centro = image.GetPixel(i, j).B

                            'Distribui os valores em um vetor para encontrar o maior
                            Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                            novaCorFinalB = valorMax(vetorCoresB)
                        Else
                            If (i = 0) Then 'Lateral esquerda da imagem

                                'Pega valores de R da lateral direita da imagem para completar-se
                                d1 = image.GetPixel(largura, j - 1).R
                                d2 = image.GetPixel(1, j - 1).R
                                l1 = image.GetPixel(largura, j).R
                                l2 = image.GetPixel(1, j).R
                                d3 = image.GetPixel(largura, 1).R
                                d4 = image.GetPixel(largura, j + 1).R
                                l3 = image.GetPixel(i, j - 1).R
                                l4 = image.GetPixel(i, j + 1).R
                                centro = image.GetPixel(i, j).R

                                'Distribui os valores em um vetor para encontrar o maior
                                Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                novaCorFinalR = valorMax(vetorCoresR)

                                'Pega valores de R da lateral direita da imagem para completar-se
                                d1 = image.GetPixel(largura, j - 1).G
                                d2 = image.GetPixel(1, j - 1).G
                                l1 = image.GetPixel(largura, j).G
                                l2 = image.GetPixel(1, j).G
                                d3 = image.GetPixel(largura, 1).G
                                d4 = image.GetPixel(largura, j + 1).G
                                l3 = image.GetPixel(i, j - 1).G
                                l4 = image.GetPixel(i, j + 1).G
                                centro = image.GetPixel(i, j).G

                                'Distribui os valores em um vetor para encontrar o maior
                                Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                novaCorFinalG = valorMax(vetorCoresG)

                                'Pega valores de R da lateral direita da imagem para completar-se
                                d1 = image.GetPixel(largura, j - 1).B
                                d2 = image.GetPixel(1, j - 1).B
                                l1 = image.GetPixel(largura, j).B
                                l2 = image.GetPixel(1, j).B
                                d3 = image.GetPixel(largura, 1).B
                                d4 = image.GetPixel(largura, j + 1).B
                                l3 = image.GetPixel(i, j - 1).B
                                l4 = image.GetPixel(i, j + 1).B
                                centro = image.GetPixel(i, j).B

                                'Distribui os valores em um vetor para encontrar o maior
                                Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                novaCorFinalB = valorMax(vetorCoresB)
                            Else
                                If (j = 0) Then 'Linha superior da imagem

                                    'Pega valores de R da linha inferior da imagem para completar-se 
                                    d1 = image.GetPixel(i - 1, altura).R
                                    d2 = image.GetPixel(i + 1, altura).R
                                    l1 = image.GetPixel(i - 1, j).R
                                    l2 = image.GetPixel(i + 1, j).R
                                    d3 = image.GetPixel(i - 1, j + 1).R
                                    d4 = image.GetPixel(i + 1, j + 1).R
                                    l3 = image.GetPixel(i, altura).R
                                    l4 = image.GetPixel(i, 1).R
                                    centro = image.GetPixel(i, j).R

                                    'Distribui os valores em um vetor para encontrar o maior
                                    Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                    novaCorFinalR = valorMax(vetorCoresR)

                                    'Pega valores de R da linha inferior da imagem para completar-se 
                                    d1 = image.GetPixel(i - 1, altura).G
                                    d2 = image.GetPixel(i + 1, altura).G
                                    l1 = image.GetPixel(i - 1, j).G
                                    l2 = image.GetPixel(i + 1, j).G
                                    d3 = image.GetPixel(i - 1, j + 1).G
                                    d4 = image.GetPixel(i + 1, j + 1).G
                                    l3 = image.GetPixel(i, altura).G
                                    l4 = image.GetPixel(i, 1).G
                                    centro = image.GetPixel(i, j).G

                                    'Distribui os valores em um vetor para encontrar o maior
                                    Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                    novaCorFinalG = valorMax(vetorCoresG)

                                    'Pega valores de R da linha inferior da imagem para completar-se 
                                    d1 = image.GetPixel(i - 1, altura).B
                                    d2 = image.GetPixel(i + 1, altura).B
                                    l1 = image.GetPixel(i - 1, j).B
                                    l2 = image.GetPixel(i + 1, j).B
                                    d3 = image.GetPixel(i - 1, j + 1).B
                                    d4 = image.GetPixel(i + 1, j + 1).B
                                    l3 = image.GetPixel(i, altura).B
                                    l4 = image.GetPixel(i, 1).B
                                    centro = image.GetPixel(i, j).B

                                    'Distribui os valores em um vetor para encontrar o maior
                                    Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                    novaCorFinalB = valorMax(vetorCoresB)
                                Else
                                    If (i = largura And j = altura) Then 'Quina inferior direita da imagem

                                        'Pega os valores de R da linha superior e da lateral esquerda da imagem
                                        dAux1 = image.GetPixel(i, 0).R
                                        dAux2 = image.GetPixel(0, j).R
                                        d1 = image.GetPixel(i - 1, j - 1).R
                                        d2 = image.GetPixel(0, j - 1).R
                                        l1 = image.GetPixel(i - 1, j).R
                                        l2 = image.GetPixel(0, j).R
                                        d3 = image.GetPixel(i - 1, 0).R
                                        d4 = Math.Round((dAux1 + dAux2) / 2)
                                        l3 = image.GetPixel(i, j - 1).R
                                        l4 = image.GetPixel(i, 0).R
                                        centro = image.GetPixel(i, j).R

                                        'Distribui os valores em um vetor para encontrar o maior
                                        Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                        novaCorFinalR = valorMax(vetorCoresR)

                                        'Pega os valores de R da linha superior e da lateral esquerda da imagem
                                        dAux1 = image.GetPixel(i, 0).G
                                        dAux2 = image.GetPixel(0, j).G
                                        d1 = image.GetPixel(i - 1, j - 1).G
                                        d2 = image.GetPixel(0, j - 1).G
                                        l1 = image.GetPixel(i - 1, j).G
                                        l2 = image.GetPixel(0, j).G
                                        d3 = image.GetPixel(i - 1, 0).G
                                        d4 = Math.Round((dAux1 + dAux2) / 2)
                                        l3 = image.GetPixel(i, j - 1).G
                                        l4 = image.GetPixel(i, 0).G
                                        centro = image.GetPixel(i, j).G

                                        'Distribui os valores em um vetor para encontrar o maior
                                        Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                        novaCorFinalG = valorMax(vetorCoresG)

                                        'Pega os valores de R da linha superior e da lateral esquerda da imagem
                                        dAux1 = image.GetPixel(i, 0).B
                                        dAux2 = image.GetPixel(0, j).B
                                        d1 = image.GetPixel(i - 1, j - 1).B
                                        d2 = image.GetPixel(0, j - 1).B
                                        l1 = image.GetPixel(i - 1, j).B
                                        l2 = image.GetPixel(0, j).B
                                        d3 = image.GetPixel(i - 1, 0).B
                                        d4 = Math.Round((dAux1 + dAux2) / 2)
                                        l3 = image.GetPixel(i, j - 1).B
                                        l4 = image.GetPixel(i, 0).B
                                        centro = image.GetPixel(i, j).B

                                        'Distribui os valores em um vetor para encontrar o maior
                                        Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                        novaCorFinalB = valorMax(vetorCoresB)
                                    Else
                                        If (i = largura) Then 'Lateral direita da imagem

                                            'Pega valores de R da lateral esquerda da imagem para completar-se
                                            d1 = image.GetPixel(i - 1, j - 1).R
                                            d2 = image.GetPixel(0, j - 1).R
                                            l1 = image.GetPixel(i - 1, j).R
                                            l2 = image.GetPixel(0, j).R
                                            d3 = image.GetPixel(i - 1, j + 1).R
                                            d4 = image.GetPixel(0, j + 1).R
                                            l3 = image.GetPixel(i, j - 1).R
                                            l4 = image.GetPixel(i, j + 1).R
                                            centro = image.GetPixel(i, j).R

                                            'Distribui os valores em um vetor para encontrar o maior
                                            Dim vetorCores() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                            novaCorFinalR = valorMax(vetorCores)

                                            'Pega valores de R da lateral esquerda da imagem para completar-se
                                            d1 = image.GetPixel(i - 1, j - 1).G
                                            d2 = image.GetPixel(0, j - 1).G
                                            l1 = image.GetPixel(i - 1, j).G
                                            l2 = image.GetPixel(0, j).G
                                            d3 = image.GetPixel(i - 1, j + 1).G
                                            d4 = image.GetPixel(0, j + 1).G
                                            l3 = image.GetPixel(i, j - 1).G
                                            l4 = image.GetPixel(i, j + 1).G
                                            centro = image.GetPixel(i, j).G

                                            'Distribui os valores em um vetor para encontrar o maior
                                            Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                            novaCorFinalG = valorMax(vetorCoresG)

                                            'Pega valores de R da lateral esquerda da imagem para completar-se
                                            d1 = image.GetPixel(i - 1, j - 1).B
                                            d2 = image.GetPixel(0, j - 1).B
                                            l1 = image.GetPixel(i - 1, j).B
                                            l2 = image.GetPixel(0, j).B
                                            d3 = image.GetPixel(i - 1, j + 1).B
                                            d4 = image.GetPixel(0, j + 1).B
                                            l3 = image.GetPixel(i, j - 1).B
                                            l4 = image.GetPixel(i, j + 1).B
                                            centro = image.GetPixel(i, j).B

                                            'Distribui os valores em um vetor para encontrar o maior
                                            Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                            novaCorFinalB = valorMax(vetorCoresB)
                                        Else
                                            If (j = altura) Then 'Linha inferior da imagem

                                                'Pega valores de R da linha superior da imagem para completar-se
                                                d1 = image.GetPixel(i - 1, j - 1).R
                                                d2 = image.GetPixel(i + 1, j - 1).R
                                                l1 = image.GetPixel(i - 1, j).R
                                                l2 = image.GetPixel(i + 1, j).R
                                                d3 = image.GetPixel(i - 1, 0).R
                                                d4 = image.GetPixel(i + 1, 0).R
                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, 0).R
                                                centro = image.GetPixel(i, j).R

                                                'Distribui os valores em um vetor para encontrar o maior
                                                Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                                novaCorFinalR = valorMax(vetorCoresR)

                                                'Pega valores de R da linha superior da imagem para completar-se
                                                d1 = image.GetPixel(i - 1, j - 1).G
                                                d2 = image.GetPixel(i + 1, j - 1).G
                                                l1 = image.GetPixel(i - 1, j).G
                                                l2 = image.GetPixel(i + 1, j).G
                                                d3 = image.GetPixel(i - 1, 0).G
                                                d4 = image.GetPixel(i + 1, 0).G
                                                l3 = image.GetPixel(i, j - 1).G
                                                l4 = image.GetPixel(i, 0).G
                                                centro = image.GetPixel(i, j).G

                                                'Distribui os valores em um vetor para encontrar o maior
                                                Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                                novaCorFinalG = valorMax(vetorCoresG)

                                                'Pega valores de R da linha superior da imagem para completar-se
                                                d1 = image.GetPixel(i - 1, j - 1).B
                                                d2 = image.GetPixel(i + 1, j - 1).B
                                                l1 = image.GetPixel(i - 1, j).B
                                                l2 = image.GetPixel(i + 1, j).B
                                                d3 = image.GetPixel(i - 1, 0).B
                                                d4 = image.GetPixel(i + 1, 0).B
                                                l3 = image.GetPixel(i, j - 1).B
                                                l4 = image.GetPixel(i, 0).B
                                                centro = image.GetPixel(i, j).B

                                                'Distribui os valores em um vetor para encontrar o maior
                                                Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                                novaCorFinalB = valorMax(vetorCoresB)
                                            Else

                                                'Caso não esteja nas bordas da imagem pega o valor de R
                                                d1 = image.GetPixel(i - 1, j - 1).R
                                                d2 = image.GetPixel(i + 1, j - 1).R
                                                l1 = image.GetPixel(i - 1, j).R
                                                l2 = image.GetPixel(i + 1, j).R
                                                d3 = image.GetPixel(i - 1, j + 1).R
                                                d4 = image.GetPixel(i + 1, j + 1).R
                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, j + 1).R
                                                centro = image.GetPixel(i, j).R

                                                'Distribui os valores em um vetor para encontrar o maior
                                                Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                                novaCorFinalR = valorMax(vetorCoresR)

                                                'Caso não esteja nas bordas da imagem pega o valor de R
                                                d1 = image.GetPixel(i - 1, j - 1).G
                                                d2 = image.GetPixel(i + 1, j - 1).G
                                                l1 = image.GetPixel(i - 1, j).G
                                                l2 = image.GetPixel(i + 1, j).G
                                                d3 = image.GetPixel(i - 1, j + 1).G
                                                d4 = image.GetPixel(i + 1, j + 1).G
                                                l3 = image.GetPixel(i, j - 1).G
                                                l4 = image.GetPixel(i, j + 1).G
                                                centro = image.GetPixel(i, j).G

                                                'Distribui os valores em um vetor para encontrar o maior
                                                Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                                novaCorFinalG = valorMax(vetorCoresG)

                                                'Caso não esteja nas bordas da imagem pega o valor de R
                                                d1 = image.GetPixel(i - 1, j - 1).B
                                                d2 = image.GetPixel(i + 1, j - 1).B
                                                l1 = image.GetPixel(i - 1, j).B
                                                l2 = image.GetPixel(i + 1, j).B
                                                d3 = image.GetPixel(i - 1, j + 1).B
                                                d4 = image.GetPixel(i + 1, j + 1).B
                                                l3 = image.GetPixel(i, j - 1).B
                                                l4 = image.GetPixel(i, j + 1).B
                                                centro = image.GetPixel(i, j).B

                                                'Distribui os valores em um vetor para encontrar o maior
                                                Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                                novaCorFinalB = valorMax(vetorCoresB)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
                copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinalR, novaCorFinalG, novaCorFinalB))

                'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1

                'Aumenta o progresso mostrado na barra de carregamento 
                progressoImagem.Value = progressoImagem.Value + 1
            Next
        Next
        Return copiaTemp
    End Function

    'Aplica o filtro de Mínimo sob uma imagem que foi passada como parâmetro
    Function filtroMinimo(image As Bitmap) As Bitmap
        Dim copiaTemp As New Bitmap(image.Width, image.Height)
        'Dim cor As Color
        Dim novaCorFinalR, novaCorFinalG, novaCorFinalB As Integer
        Dim d1, d2, d3, d4, dAux1, dAux2 As Integer
        Dim l1, l2, l3, l4, centro As Integer
        Dim altura As Integer = image.Height - 1
        Dim largura As Integer = image.Width - 1
        'd1   l3   d2
        'l1 centro l2
        'd3   l4   d4

        For i = 0 To largura
            For j = 0 To altura
                If (i = 0 And j = 0) Then 'Quina superior esquerda da imagem

                    'Pega valores de R da linha inferior e da lateral direita da imagem para completar-se
                    dAux1 = image.GetPixel(0, altura).R
                    dAux2 = image.GetPixel(largura, 0).R
                    d1 = Math.Round((dAux1 + dAux2) / 2)
                    d2 = image.GetPixel(1, altura).R
                    l1 = image.GetPixel(largura, 1).R
                    l2 = image.GetPixel(1, 0).R
                    d3 = image.GetPixel(largura, 1).R
                    d4 = image.GetPixel(1, 1).R
                    l3 = dAux1
                    l4 = image.GetPixel(0, 1).R
                    centro = image.GetPixel(i, j).R

                    'Distribui os valores em um vetor para encontrar o menor
                    Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                    novaCorFinalR = valorMin(vetorCoresR)

                    'Pega valores de R da linha inferior e da lateral direita da imagem para completar-se
                    dAux1 = image.GetPixel(0, altura).G
                    dAux2 = image.GetPixel(largura, 0).G
                    d1 = Math.Round((dAux1 + dAux2) / 2)
                    d2 = image.GetPixel(1, altura).G
                    l1 = image.GetPixel(largura, 1).G
                    l2 = image.GetPixel(1, 0).G
                    d3 = image.GetPixel(largura, 1).G
                    d4 = image.GetPixel(1, 1).G
                    l3 = dAux1
                    l4 = image.GetPixel(0, 1).G
                    centro = image.GetPixel(i, j).G

                    'Distribui os valores em um vetor para encontrar o menor
                    Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                    novaCorFinalG = valorMin(vetorCoresG)

                    'Pega valores de R da linha inferior e da lateral direita da imagem para completar-se
                    dAux1 = image.GetPixel(0, altura).B
                    dAux2 = image.GetPixel(largura, 0).B
                    d1 = Math.Round((dAux1 + dAux2) / 2)
                    d2 = image.GetPixel(1, altura).B
                    l1 = image.GetPixel(largura, 1).B
                    l2 = image.GetPixel(1, 0).B
                    d3 = image.GetPixel(largura, 1).B
                    d4 = image.GetPixel(1, 1).B
                    l3 = dAux1
                    l4 = image.GetPixel(0, 1).B
                    centro = image.GetPixel(i, j).B

                    'Distribui os valores em um vetor para encontrar o menor
                    Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                    novaCorFinalB = valorMin(vetorCoresB)
                Else

                    If (i = 0 And j = altura) Then 'Quina inferior esquerda

                        'Pega valores de R linha superior e da lateral direita da imagem para completar-se
                        dAux1 = image.GetPixel(largura, j).R
                        dAux2 = image.GetPixel(1, 0).R
                        d1 = image.GetPixel(largura, j - 1).R
                        d2 = image.GetPixel(1, j - 1).R
                        l1 = image.GetPixel(largura, j).R
                        l2 = image.GetPixel(1, j).R
                        d3 = Math.Round((dAux1 + dAux2) / 2)
                        d4 = image.GetPixel(1, 0).R
                        l3 = image.GetPixel(i, j - 1).R
                        l4 = image.GetPixel(0, 0).R
                        centro = image.GetPixel(i, j).R

                        'Distribui os valores em um vetor para encontrar o menor
                        Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                        novaCorFinalR = valorMin(vetorCoresR)

                        'Pega valores de R linha superior e da lateral direita da imagem para completar-se
                        dAux1 = image.GetPixel(largura, j).G
                        dAux2 = image.GetPixel(1, 0).G
                        d1 = image.GetPixel(largura, j - 1).G
                        d2 = image.GetPixel(1, j - 1).G
                        l1 = image.GetPixel(largura, j).G
                        l2 = image.GetPixel(1, j).G
                        d3 = Math.Round((dAux1 + dAux2) / 2)
                        d4 = image.GetPixel(1, 0).G
                        l3 = image.GetPixel(i, j - 1).G
                        l4 = image.GetPixel(0, 0).G
                        centro = image.GetPixel(i, j).G

                        'Distribui os valores em um vetor para encontrar o menor
                        Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                        novaCorFinalG = valorMin(vetorCoresG)

                        'Pega valores de R linha superior e da lateral direita da imagem para completar-se
                        dAux1 = image.GetPixel(largura, j).B
                        dAux2 = image.GetPixel(1, 0).B
                        d1 = image.GetPixel(largura, j - 1).B
                        d2 = image.GetPixel(1, j - 1).B
                        l1 = image.GetPixel(largura, j).B
                        l2 = image.GetPixel(1, j).B
                        d3 = Math.Round((dAux1 + dAux2) / 2)
                        d4 = image.GetPixel(1, 0).B
                        l3 = image.GetPixel(i, j - 1).B
                        l4 = image.GetPixel(0, 0).B
                        centro = image.GetPixel(i, j).B

                        'Distribui os valores em um vetor para encontrar o menor
                        Dim vetorCores() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                        novaCorFinalB = valorMin(vetorCores)
                    Else
                        If (i = largura And j = 0) Then 'Quina superior direita da imagem

                            'Pega valores de R da linha inferior e lateral esquerda para completar-se
                            dAux1 = image.GetPixel(i, altura).R
                            dAux2 = image.GetPixel(0, 0).R
                            d1 = image.GetPixel(i - 1, altura).R
                            d2 = Math.Round((dAux1 + dAux2) / 2)
                            l1 = image.GetPixel(i, 0).R
                            l2 = image.GetPixel(0, 0).R
                            d3 = image.GetPixel(i - 1, 1).R
                            d4 = image.GetPixel(0, 1).R
                            l3 = image.GetPixel(i, altura).R
                            l4 = image.GetPixel(i, 1).R
                            centro = image.GetPixel(i, j).R

                            'Distribui os valores em um vetor para encontrar o menor
                            Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                            novaCorFinalR = valorMin(vetorCoresR)

                            'Pega valores de R da linha inferior e lateral esquerda para completar-se
                            dAux1 = image.GetPixel(i, altura).G
                            dAux2 = image.GetPixel(0, 0).G
                            d1 = image.GetPixel(i - 1, altura).G
                            d2 = Math.Round((dAux1 + dAux2) / 2)
                            l1 = image.GetPixel(i, 0).G
                            l2 = image.GetPixel(0, 0).G
                            d3 = image.GetPixel(i - 1, 1).G
                            d4 = image.GetPixel(0, 1).G
                            l3 = image.GetPixel(i, altura).G
                            l4 = image.GetPixel(i, 1).G
                            centro = image.GetPixel(i, j).G

                            'Distribui os valores em um vetor para encontrar o menor
                            Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                            novaCorFinalG = valorMin(vetorCoresG)

                            'Pega valores de R da linha inferior e lateral esquerda para completar-se
                            dAux1 = image.GetPixel(i, altura).B
                            dAux2 = image.GetPixel(0, 0).B
                            d1 = image.GetPixel(i - 1, altura).B
                            d2 = Math.Round((dAux1 + dAux2) / 2)
                            l1 = image.GetPixel(i, 0).B
                            l2 = image.GetPixel(0, 0).B
                            d3 = image.GetPixel(i - 1, 1).B
                            d4 = image.GetPixel(0, 1).B
                            l3 = image.GetPixel(i, altura).B
                            l4 = image.GetPixel(i, 1).B
                            centro = image.GetPixel(i, j).B

                            'Distribui os valores em um vetor para encontrar o menor
                            Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                            novaCorFinalB = valorMin(vetorCoresB)
                        Else
                            If (i = 0) Then 'Lateral esquerda da imagem

                                'Pega valores de R da lateral direita da imagem para completar-se
                                d1 = image.GetPixel(largura, j - 1).R
                                d2 = image.GetPixel(1, j - 1).R
                                l1 = image.GetPixel(largura, j).R
                                l2 = image.GetPixel(1, j).R
                                d3 = image.GetPixel(largura, 1).R
                                d4 = image.GetPixel(largura, j + 1).R
                                l3 = image.GetPixel(i, j - 1).R
                                l4 = image.GetPixel(i, j + 1).R
                                centro = image.GetPixel(i, j).R

                                'Distribui os valores em um vetor para encontrar o menor
                                Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                novaCorFinalR = valorMin(vetorCoresR)

                                'Pega valores de R da lateral direita da imagem para completar-se
                                d1 = image.GetPixel(largura, j - 1).G
                                d2 = image.GetPixel(1, j - 1).G
                                l1 = image.GetPixel(largura, j).G
                                l2 = image.GetPixel(1, j).G
                                d3 = image.GetPixel(largura, 1).G
                                d4 = image.GetPixel(largura, j + 1).G
                                l3 = image.GetPixel(i, j - 1).G
                                l4 = image.GetPixel(i, j + 1).G
                                centro = image.GetPixel(i, j).G

                                'Distribui os valores em um vetor para encontrar o menor
                                Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                novaCorFinalG = valorMin(vetorCoresG)

                                'Pega valores de R da lateral direita da imagem para completar-se
                                d1 = image.GetPixel(largura, j - 1).B
                                d2 = image.GetPixel(1, j - 1).B
                                l1 = image.GetPixel(largura, j).B
                                l2 = image.GetPixel(1, j).B
                                d3 = image.GetPixel(largura, 1).B
                                d4 = image.GetPixel(largura, j + 1).B
                                l3 = image.GetPixel(i, j - 1).B
                                l4 = image.GetPixel(i, j + 1).B
                                centro = image.GetPixel(i, j).B

                                'Distribui os valores em um vetor para encontrar o menor
                                Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                novaCorFinalB = valorMin(vetorCoresB)
                            Else
                                If (j = 0) Then 'Linha superior da imagem

                                    'Pega valores de R da linha inferior da imagem para completar-se 
                                    d1 = image.GetPixel(i - 1, altura).R
                                    d2 = image.GetPixel(i + 1, altura).R
                                    l1 = image.GetPixel(i - 1, j).R
                                    l2 = image.GetPixel(i + 1, j).R
                                    d3 = image.GetPixel(i - 1, j + 1).R
                                    d4 = image.GetPixel(i + 1, j + 1).R
                                    l3 = image.GetPixel(i, altura).R
                                    l4 = image.GetPixel(i, 1).R
                                    centro = image.GetPixel(i, j).R

                                    'Distribui os valores em um vetor para encontrar o menor
                                    Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                    novaCorFinalR = valorMin(vetorCoresR)

                                    'Pega valores de R da linha inferior da imagem para completar-se 
                                    d1 = image.GetPixel(i - 1, altura).G
                                    d2 = image.GetPixel(i + 1, altura).G
                                    l1 = image.GetPixel(i - 1, j).G
                                    l2 = image.GetPixel(i + 1, j).G
                                    d3 = image.GetPixel(i - 1, j + 1).G
                                    d4 = image.GetPixel(i + 1, j + 1).G
                                    l3 = image.GetPixel(i, altura).G
                                    l4 = image.GetPixel(i, 1).G
                                    centro = image.GetPixel(i, j).G

                                    'Distribui os valores em um vetor para encontrar o menor
                                    Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                    novaCorFinalG = valorMin(vetorCoresG)

                                    'Pega valores de R da linha inferior da imagem para completar-se 
                                    d1 = image.GetPixel(i - 1, altura).B
                                    d2 = image.GetPixel(i + 1, altura).B
                                    l1 = image.GetPixel(i - 1, j).B
                                    l2 = image.GetPixel(i + 1, j).B
                                    d3 = image.GetPixel(i - 1, j + 1).B
                                    d4 = image.GetPixel(i + 1, j + 1).B
                                    l3 = image.GetPixel(i, altura).B
                                    l4 = image.GetPixel(i, 1).B
                                    centro = image.GetPixel(i, j).B

                                    'Distribui os valores em um vetor para encontrar o menor
                                    Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                    novaCorFinalB = valorMin(vetorCoresB)
                                Else
                                    If (i = largura And j = altura) Then 'Quina inferior direita da imagem

                                        'Pega os valores de R da linha superior e da lateral esquerda da imagem
                                        dAux1 = image.GetPixel(i, 0).R
                                        dAux2 = image.GetPixel(0, j).R
                                        d1 = image.GetPixel(i - 1, j - 1).R
                                        d2 = image.GetPixel(0, j - 1).R
                                        l1 = image.GetPixel(i - 1, j).R
                                        l2 = image.GetPixel(0, j).R
                                        d3 = image.GetPixel(i - 1, 0).R
                                        d4 = Math.Round((dAux1 + dAux2) / 2)
                                        l3 = image.GetPixel(i, j - 1).R
                                        l4 = image.GetPixel(i, 0).R
                                        centro = image.GetPixel(i, j).R

                                        'Distribui os valores em um vetor para encontrar o menor
                                        Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                        novaCorFinalR = valorMin(vetorCoresR)

                                        'Pega os valores de R da linha superior e da lateral esquerda da imagem
                                        dAux1 = image.GetPixel(i, 0).G
                                        dAux2 = image.GetPixel(0, j).G
                                        d1 = image.GetPixel(i - 1, j - 1).G
                                        d2 = image.GetPixel(0, j - 1).G
                                        l1 = image.GetPixel(i - 1, j).G
                                        l2 = image.GetPixel(0, j).G
                                        d3 = image.GetPixel(i - 1, 0).G
                                        d4 = Math.Round((dAux1 + dAux2) / 2)
                                        l3 = image.GetPixel(i, j - 1).G
                                        l4 = image.GetPixel(i, 0).G
                                        centro = image.GetPixel(i, j).G

                                        'Distribui os valores em um vetor para encontrar o menor
                                        Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                        novaCorFinalG = valorMin(vetorCoresG)

                                        'Pega os valores de R da linha superior e da lateral esquerda da imagem
                                        dAux1 = image.GetPixel(i, 0).B
                                        dAux2 = image.GetPixel(0, j).B
                                        d1 = image.GetPixel(i - 1, j - 1).B
                                        d2 = image.GetPixel(0, j - 1).B
                                        l1 = image.GetPixel(i - 1, j).B
                                        l2 = image.GetPixel(0, j).B
                                        d3 = image.GetPixel(i - 1, 0).B
                                        d4 = Math.Round((dAux1 + dAux2) / 2)
                                        l3 = image.GetPixel(i, j - 1).B
                                        l4 = image.GetPixel(i, 0).B
                                        centro = image.GetPixel(i, j).B

                                        'Distribui os valores em um vetor para encontrar o menor
                                        Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                        novaCorFinalB = valorMin(vetorCoresB)
                                    Else
                                        If (i = largura) Then 'Lateral direita da imagem

                                            'Pega valores de R da lateral esquerda da imagem para completar-se
                                            d1 = image.GetPixel(i - 1, j - 1).R
                                            d2 = image.GetPixel(0, j - 1).R
                                            l1 = image.GetPixel(i - 1, j).R
                                            l2 = image.GetPixel(0, j).R
                                            d3 = image.GetPixel(i - 1, j + 1).R
                                            d4 = image.GetPixel(0, j + 1).R
                                            l3 = image.GetPixel(i, j - 1).R
                                            l4 = image.GetPixel(i, j + 1).R
                                            centro = image.GetPixel(i, j).R

                                            'Distribui os valores em um vetor para encontrar o menor
                                            Dim vetorCores() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                            novaCorFinalR = valorMin(vetorCores)

                                            'Pega valores de R da lateral esquerda da imagem para completar-se
                                            d1 = image.GetPixel(i - 1, j - 1).G
                                            d2 = image.GetPixel(0, j - 1).G
                                            l1 = image.GetPixel(i - 1, j).G
                                            l2 = image.GetPixel(0, j).G
                                            d3 = image.GetPixel(i - 1, j + 1).G
                                            d4 = image.GetPixel(0, j + 1).G
                                            l3 = image.GetPixel(i, j - 1).G
                                            l4 = image.GetPixel(i, j + 1).G
                                            centro = image.GetPixel(i, j).G

                                            'Distribui os valores em um vetor para encontrar o menor
                                            Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                            novaCorFinalG = valorMin(vetorCoresG)

                                            'Pega valores de R da lateral esquerda da imagem para completar-se
                                            d1 = image.GetPixel(i - 1, j - 1).B
                                            d2 = image.GetPixel(0, j - 1).B
                                            l1 = image.GetPixel(i - 1, j).B
                                            l2 = image.GetPixel(0, j).B
                                            d3 = image.GetPixel(i - 1, j + 1).B
                                            d4 = image.GetPixel(0, j + 1).B
                                            l3 = image.GetPixel(i, j - 1).B
                                            l4 = image.GetPixel(i, j + 1).B
                                            centro = image.GetPixel(i, j).B

                                            'Distribui os valores em um vetor para encontrar o menor
                                            Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                            novaCorFinalB = valorMin(vetorCoresB)
                                        Else
                                            If (j = altura) Then 'Linha inferior da imagem

                                                'Pega valores de R da linha superior da imagem para completar-se
                                                d1 = image.GetPixel(i - 1, j - 1).R
                                                d2 = image.GetPixel(i + 1, j - 1).R
                                                l1 = image.GetPixel(i - 1, j).R
                                                l2 = image.GetPixel(i + 1, j).R
                                                d3 = image.GetPixel(i - 1, 0).R
                                                d4 = image.GetPixel(i + 1, 0).R
                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, 0).R
                                                centro = image.GetPixel(i, j).R

                                                'Distribui os valores em um vetor para encontrar o menor
                                                Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                                novaCorFinalR = valorMin(vetorCoresR)

                                                'Pega valores de R da linha superior da imagem para completar-se
                                                d1 = image.GetPixel(i - 1, j - 1).G
                                                d2 = image.GetPixel(i + 1, j - 1).G
                                                l1 = image.GetPixel(i - 1, j).G
                                                l2 = image.GetPixel(i + 1, j).G
                                                d3 = image.GetPixel(i - 1, 0).G
                                                d4 = image.GetPixel(i + 1, 0).G
                                                l3 = image.GetPixel(i, j - 1).G
                                                l4 = image.GetPixel(i, 0).G
                                                centro = image.GetPixel(i, j).G

                                                'Distribui os valores em um vetor para encontrar o menor
                                                Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                                novaCorFinalG = valorMin(vetorCoresG)

                                                'Pega valores de R da linha superior da imagem para completar-se
                                                d1 = image.GetPixel(i - 1, j - 1).B
                                                d2 = image.GetPixel(i + 1, j - 1).B
                                                l1 = image.GetPixel(i - 1, j).B
                                                l2 = image.GetPixel(i + 1, j).B
                                                d3 = image.GetPixel(i - 1, 0).B
                                                d4 = image.GetPixel(i + 1, 0).B
                                                l3 = image.GetPixel(i, j - 1).B
                                                l4 = image.GetPixel(i, 0).B
                                                centro = image.GetPixel(i, j).B

                                                'Distribui os valores em um vetor para encontrar o menor
                                                Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                                novaCorFinalB = valorMin(vetorCoresB)
                                            Else

                                                'Caso não esteja nas bordas da imagem pega o valor de R
                                                d1 = image.GetPixel(i - 1, j - 1).R
                                                d2 = image.GetPixel(i + 1, j - 1).R
                                                l1 = image.GetPixel(i - 1, j).R
                                                l2 = image.GetPixel(i + 1, j).R
                                                d3 = image.GetPixel(i - 1, j + 1).R
                                                d4 = image.GetPixel(i + 1, j + 1).R
                                                l3 = image.GetPixel(i, j - 1).R
                                                l4 = image.GetPixel(i, j + 1).R
                                                centro = image.GetPixel(i, j).R

                                                'Distribui os valores em um vetor para encontrar o menor
                                                Dim vetorCoresR() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                                novaCorFinalR = valorMin(vetorCoresR)

                                                'Caso não esteja nas bordas da imagem pega o valor de R
                                                d1 = image.GetPixel(i - 1, j - 1).G
                                                d2 = image.GetPixel(i + 1, j - 1).G
                                                l1 = image.GetPixel(i - 1, j).G
                                                l2 = image.GetPixel(i + 1, j).G
                                                d3 = image.GetPixel(i - 1, j + 1).G
                                                d4 = image.GetPixel(i + 1, j + 1).G
                                                l3 = image.GetPixel(i, j - 1).G
                                                l4 = image.GetPixel(i, j + 1).G
                                                centro = image.GetPixel(i, j).G

                                                'Distribui os valores em um vetor para encontrar o menor
                                                Dim vetorCoresG() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                                novaCorFinalG = valorMin(vetorCoresG)

                                                'Caso não esteja nas bordas da imagem pega o valor de R
                                                d1 = image.GetPixel(i - 1, j - 1).B
                                                d2 = image.GetPixel(i + 1, j - 1).B
                                                l1 = image.GetPixel(i - 1, j).B
                                                l2 = image.GetPixel(i + 1, j).B
                                                d3 = image.GetPixel(i - 1, j + 1).B
                                                d4 = image.GetPixel(i + 1, j + 1).B
                                                l3 = image.GetPixel(i, j - 1).B
                                                l4 = image.GetPixel(i, j + 1).B
                                                centro = image.GetPixel(i, j).B

                                                'Distribui os valores em um vetor para encontrar o menor
                                                Dim vetorCoresB() As Integer = {d1, d2, d3, d4, l1, l2, l3, l4, centro}
                                                novaCorFinalB = valorMin(vetorCoresB)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
                copiaTemp.SetPixel(i, j, Color.FromArgb(novaCorFinalR, novaCorFinalG, novaCorFinalB))

                'Guardando dados para o histograma pulando uma casa pois o grafico não mapeia se o eixo X for 0
                historicoHistograma(idImagens).quantidadePorIntensidadeR(novaCorFinalR + 1) += 1
                historicoHistograma(idImagens).quantidadePorIntensidadeG(novaCorFinalG + 1) += 1
                historicoHistograma(idImagens).quantidadePorIntensidadeB(novaCorFinalB + 1) += 1

                'Aumenta o progresso mostrado na barra de carregamento 
                progressoImagem.Value = progressoImagem.Value + 1
            Next
        Next
        Return copiaTemp
    End Function

    'Aplica o filtro de Abertura sob uma imagem que foi passada como parâmetro
    Function filtroAbertura(image As Bitmap) As Bitmap
        Dim copiaTemp As New Bitmap(image.Width, image.Height)

        'O filtro é uma junção do filtro de Máximo com o de Mínimo, um aplicado após o outro em uma imagem
        copiaTemp = filtroMinimo(copiaTemp)
        copiaTemp = filtroMaximo(image)

        Return copiaTemp
    End Function

    'Aplica o filtro de Fechamento sob uma imagem que foi passada como parâmetro
    Function filtroFechamento(image As Bitmap) As Bitmap
        Dim copiaTemp As New Bitmap(image.Width, image.Height)

        'O filtro é uma junção do filtro de Mpinimo com o de Máximo, um aplicado após o outro em uma imagem
        copiaTemp = filtroMaximo(copiaTemp)
        copiaTemp = filtroMinimo(image)

        Return copiaTemp
    End Function

    '-----------------------------------------------------------------------------------------------------------------------------------------------------
    '-----------------------------------FUNÇÕES DE ORDENAÇÃO, REDIMENSIONAMENTO E APOIO GERAL PARA AS OPERAÇÕES DO PROGRAMA-------------------------------

    'Função para encontrar o maior valor de um vetor
    Function valorMax(vetor() As Integer) As Integer
        Dim maior As Integer = 0
        For i = 0 To vetor.Length - 1
            If (vetor(i) > maior) Then
                maior = vetor(i)
            End If
        Next
        Return maior
    End Function

    'Função para encontrar o menor valor de um vetor
    Function valorMin(vetor() As Integer) As Integer
        Dim menor As Integer = 300
        For i = 0 To vetor.Length - 1
            If (vetor(i) < menor) Then
                menor = vetor(i)
            End If
        Next
        Return menor
    End Function

    'Função para encontrar o valor mediano de um vetor
    Function valorMediano(vetor() As Integer) As Integer
        Dim aux As Integer
        For i = 0 To vetor.Length - 2
            For j = i + 1 To vetor.Length - 1
                If (vetor(i) > vetor(j)) Then
                    aux = vetor(i)
                    vetor(i) = vetor(j)
                    vetor(j) = aux
                End If
            Next
        Next
        Return vetor(5)
    End Function

    'Preenche os  vetores de intensidades com as quantidades de pixels correspondentes à cada intensidade dada nas faixa R, G e B
    Function quantizaIntensidades(image As Bitmap) As Histograma
        Dim altura As Integer = image.Height - 1
        Dim largura As Integer = image.Width - 1

        Dim dados As Histograma = New Histograma()
        'para lidar com imagens coloridas, serão pegos os valores de R, G e B separadamente
        For i = 0 To largura
            For j = 0 To altura
                dados.quantidadePorIntensidadeR(image.GetPixel(i, j).R) += 1
                dados.quantidadePorIntensidadeG(image.GetPixel(i, j).G) += 1
                dados.quantidadePorIntensidadeB(image.GetPixel(i, j).B) += 1

                'Aumenta o progresso mostrado na barra de carregamento 
                progressoImagem.Value = progressoImagem.Value + 1
            Next
        Next

        Return dados
    End Function

    'Reduz o tamanho da imagem original,caso ela seja maior que Full HD, para que ela possa ser processada. Imagens com dimensões maiores que full HD causam gargalo.
    Function reduzTamanhoImagem(imagem As Image) As Image

        If (imagem.Height > imagem.Width) Then
            Dim altura As Integer = 1920
            Dim largura As Integer = Math.Round(imagem.Width / (imagem.Height / 1920))

            Dim bmpTemp As New Bitmap(largura, altura)

            Dim cg As Graphics = Graphics.FromImage(bmpTemp)
            cg.DrawImage(imagem, New Rectangle(0, 0, largura, altura), 0, 0, imagem.Width, imagem.Height, GraphicsUnit.Pixel)
            cg.Dispose()

            Return bmpTemp
        Else
            Dim altura As Integer = Math.Round(imagem.Height / (imagem.Width / 1920))
            Dim largura As Integer = 1920

            Dim bmpTemp As New Bitmap(largura, altura)

            Dim cg As Graphics = Graphics.FromImage(bmpTemp)
            cg.DrawImage(imagem, New Rectangle(0, 0, largura, altura), 0, 0, imagem.Width, imagem.Height, GraphicsUnit.Pixel)
            cg.Dispose()

            Return bmpTemp
        End If

    End Function

    'Redimensiona a imagem original para caber no quadro
    Function redimensiona(imagem As Image) As Image

        Dim bmpTemp As New Bitmap(frameFoto1.Width, frameFoto1.Height)

        If (imagem.Height > 400 Or imagem.Width > 600) Then

            Dim cg As Graphics = Graphics.FromImage(bmpTemp)
            cg.DrawImage(imagem, New Rectangle(0, 0, frameFoto1.Width, frameFoto1.Height), 0, 0, imagem.Width, imagem.Height, GraphicsUnit.Pixel)
            cg.Dispose()
            Return bmpTemp
        Else
            Return imagem
        End If

    End Function

    'Cria uma imagem secundária com dimensões apropriadas para utilização de uma máscara 5x5
    Function geraMatrizPara3x3(image As Bitmap) As Bitmap
        Dim imagemRedimensionada As New Bitmap(image.Width + 2, image.Height + 2)
        Dim altura As Integer = imagemRedimensionada.Height - 1
        Dim largura As Integer = imagemRedimensionada.Width - 1

        'Preenche a imagem secundária replicando as linhas mais externas da imagem original nas linhas da imagem redimensionada que passam 
        'dos limites da imagem original
        For i = 0 To largura
            For j = 0 To altura
                If (i >= 1 And i <= largura - 1 And j >= 1 And j <= altura - 1) Then
                    imagemRedimensionada.SetPixel(i, j, image.GetPixel(i - 1, j - 1))
                Else
                    'Replica a linha superior
                    If (i >= 1 And i <= largura - 1 And j = 0) Then
                        imagemRedimensionada.SetPixel(i, j, image.GetPixel(i - 1, j))
                    Else
                        'Replica a linha inferior
                        If (i >= 1 And i <= largura - 1 And j >= altura) Then
                            imagemRedimensionada.SetPixel(i, j, image.GetPixel(i - 1, j - 3))
                        Else
                            'Replica a linha lateral da esquerda
                            If (i = 0 And j >= 1 And j <= altura - 1) Then
                                imagemRedimensionada.SetPixel(i, j, image.GetPixel(i, j - 1))
                            Else
                                'Replica a linha lateral da direita
                                If (i >= largura And j >= 1 And j <= altura - 1) Then
                                    imagemRedimensionada.SetPixel(i, j, image.GetPixel(i - 3, j - 1))
                                Else
                                    'Coloca o valor 0 nas diagonais externas
                                    imagemRedimensionada.SetPixel(i, j, Color.FromArgb(0, 0, 0))
                                End If
                            End If
                        End If
                    End If
                End If

                'Aumenta o progresso mostrado na barra de carregamento 
                progressoImagem.Value = progressoImagem.Value + 1
            Next
        Next

        Return imagemRedimensionada
    End Function

    'Cria uma imagem secundária com dimensões apropriadas para utilização de uma máscara 5x5
    Function geraMatrizPara5x5(image As Bitmap) As Bitmap
        Dim imagemRedimensionada As New Bitmap(image.Width + 4, image.Height + 4)
        Dim altura As Integer = imagemRedimensionada.Height - 1
        Dim largura As Integer = imagemRedimensionada.Width - 1

        'Preenche a imagem secundária replicando as linhas mais externas da imagem original nas linhas da imagem redimensionada que passam 
        'dos limites da imagem original
        For i = 0 To largura
            For j = 0 To altura
                If (i >= 2 And i <= largura - 2 And j >= 2 And j <= altura - 2) Then
                    imagemRedimensionada.SetPixel(i, j, image.GetPixel(i - 2, j - 2))
                Else
                    'Replica as 2 linhas superiores
                    If (i >= 2 And i <= largura - 2 And j <= 1) Then
                        imagemRedimensionada.SetPixel(i, j, image.GetPixel(i - 2, j))
                    Else
                        'Replica as 2 linhas inferiores
                        If (i >= 2 And i <= largura - 2 And j >= altura - 1) Then
                            imagemRedimensionada.SetPixel(i, j, image.GetPixel(i - 2, j - 4))
                        Else
                            'Replica as 2 linhas laterais da esquerda
                            If (i <= 1 And j >= 2 And j <= altura - 2) Then
                                imagemRedimensionada.SetPixel(i, j, image.GetPixel(i, j - 2))
                            Else
                                'Replica as 2 linhas laterais da direita
                                If (i >= largura - 1 And j >= 2 And j <= altura - 2) Then
                                    imagemRedimensionada.SetPixel(i, j, image.GetPixel(i - 4, j - 2))
                                Else
                                    'Coloca o valor 0 nas diagonais externas
                                    imagemRedimensionada.SetPixel(i, j, Color.FromArgb(0, 0, 0))
                                End If
                            End If
                        End If
                    End If
                End If

                'Aumenta o progresso mostrado na barra de carregamento 
                progressoImagem.Value = progressoImagem.Value + 1
            Next
        Next

        Return imagemRedimensionada
    End Function

    'Cria uma imagem secundária com dimensões apropriadas para utilização de uma máscara 7x7
    Function geraMatrizPara7x7(image As Bitmap) As Bitmap
        Dim imagemRedimensionada As New Bitmap(image.Width + 6, image.Height + 6)
        Dim altura As Integer = imagemRedimensionada.Height - 1
        Dim largura As Integer = imagemRedimensionada.Width - 1

        'Preenche a imagem secundária replicando as linhas mais externas da imagem original nas linhas da imagem redimensionada que passam 
        'dos limites da imagem original
        For i = 0 To largura
            For j = 0 To altura
                If (i >= 3 And i <= largura - 3 And j >= 3 And j <= altura - 3) Then
                    imagemRedimensionada.SetPixel(i, j, image.GetPixel(i - 3, j - 3))
                Else
                    'Replica as 2 linhas superiores
                    If (i >= 3 And i <= largura - 3 And j <= 2) Then
                        imagemRedimensionada.SetPixel(i, j, image.GetPixel(i - 3, j))
                    Else
                        'Replica as 2 linhas inferiores
                        If (i >= 3 And i <= largura - 3 And j >= altura - 3) Then
                            imagemRedimensionada.SetPixel(i, j, image.GetPixel(i - 3, j - 6))
                        Else
                            'Replica as 2 linhas laterais da esquerda
                            If (i <= 3 And j >= 3 And j <= altura - 3) Then
                                imagemRedimensionada.SetPixel(i, j, image.GetPixel(i, j - 3))
                            Else
                                'Replica as 2 linhas laterais da direita
                                If (i >= largura - 2 And j >= 3 And j <= altura - 3) Then
                                    imagemRedimensionada.SetPixel(i, j, image.GetPixel(i - 6, j - 3))
                                Else
                                    'Coloca o valor 0 nas diagonais externas
                                    imagemRedimensionada.SetPixel(i, j, Color.FromArgb(0, 0, 0))
                                End If
                            End If
                        End If
                    End If
                End If

                'Aumenta o progresso mostrado na barra de carregamento 
                progressoImagem.Value = progressoImagem.Value + 1
            Next
        Next

        Return imagemRedimensionada
    End Function

    Function desativaBotões()
        novaImagem.Enabled = False
        desfazer.Enabled = False
        reciclar.Enabled = False
        tonsDeCinza.Enabled = False
        negativo.Enabled = False
        binarizar.Enabled = False
        sobel.Enabled = False
        laplace.Enabled = False
        highBoost.Enabled = False
        pesosHighBoost.Enabled = False
        minimo.Enabled = False
        maximo.Enabled = False
        abertura.Enabled = False
        fechamento.Enabled = False
        media.Enabled = False
        mediana.Enabled = False
        gaussiano.Enabled = False
        dimensoesMascara.Enabled = False
        tabelaHistorico.Enabled = False

    End Function

    Function ativaBotões()
        novaImagem.Enabled = True
        desfazer.Enabled = True
        reciclar.Enabled = True
        tonsDeCinza.Enabled = True
        negativo.Enabled = True
        binarizar.Enabled = True
        sobel.Enabled = True
        laplace.Enabled = True
        highBoost.Enabled = True
        pesosHighBoost.Enabled = True
        minimo.Enabled = True
        maximo.Enabled = True
        abertura.Enabled = True
        fechamento.Enabled = True
        media.Enabled = True
        mediana.Enabled = True
        gaussiano.Enabled = True
        dimensoesMascara.Enabled = True
        tabelaHistorico.Enabled = True

        MessageBox.Show("Imagem alterada com sucesso!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Function
End Class

'--------------------------------------------------------------------------------------------------------------------------------------------------------
'-----------------------------------CLASSE DE HISTOGRAMA, PARA ORGANIZAR E PADRONIZAR A FGERAÇÃO DOS MESMOS----------------------------------------------

Public Class Histograma
    Public quantidadePorIntensidadeR() As Integer
    Public quantidadePorIntensidadeG() As Integer
    Public quantidadePorIntensidadeB() As Integer

    Public Sub New()
        quantidadePorIntensidadeR = New Integer(256) {}
        quantidadePorIntensidadeG = New Integer(256) {}
        quantidadePorIntensidadeB = New Integer(256) {}
    End Sub
End Class
