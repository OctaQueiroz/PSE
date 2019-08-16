<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TelaVisualizacaoHistograma
    Inherits System.Windows.Forms.Form

    'Descartar substituições de formulário para limpar a lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Exigido pelo Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'OBSERVAÇÃO: o procedimento a seguir é exigido pelo Windows Form Designer
    'Pode ser modificado usando o Windows Form Designer.  
    'Não o modifique usando o editor de códigos.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim ChartArea4 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend4 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series4 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim ChartArea5 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend5 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series5 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Dim ChartArea6 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend6 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series6 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Me.histogramaImagemR = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.histogramaImagemG = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.histogramaImagemB = New System.Windows.Forms.DataVisualization.Charting.Chart()
        CType(Me.histogramaImagemR, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.histogramaImagemG, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.histogramaImagemB, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'histogramaImagemR
        '
        Me.histogramaImagemR.BackColor = System.Drawing.Color.WhiteSmoke
        ChartArea4.Name = "ChartArea1"
        Me.histogramaImagemR.ChartAreas.Add(ChartArea4)
        Legend4.Enabled = False
        Legend4.Name = "Legend1"
        Me.histogramaImagemR.Legends.Add(Legend4)
        Me.histogramaImagemR.Location = New System.Drawing.Point(12, 12)
        Me.histogramaImagemR.Name = "histogramaImagemR"
        Series4.ChartArea = "ChartArea1"
        Series4.Legend = "Legend1"
        Series4.Name = "Series1"
        Me.histogramaImagemR.Series.Add(Series4)
        Me.histogramaImagemR.Size = New System.Drawing.Size(375, 279)
        Me.histogramaImagemR.TabIndex = 29
        Me.histogramaImagemR.Text = "Histograma"
        '
        'histogramaImagemG
        '
        Me.histogramaImagemG.BackColor = System.Drawing.Color.WhiteSmoke
        ChartArea5.Name = "ChartArea1"
        Me.histogramaImagemG.ChartAreas.Add(ChartArea5)
        Legend5.Enabled = False
        Legend5.Name = "Legend1"
        Me.histogramaImagemG.Legends.Add(Legend5)
        Me.histogramaImagemG.Location = New System.Drawing.Point(473, 12)
        Me.histogramaImagemG.Name = "histogramaImagemG"
        Series5.ChartArea = "ChartArea1"
        Series5.Legend = "Legend1"
        Series5.Name = "Series1"
        Me.histogramaImagemG.Series.Add(Series5)
        Me.histogramaImagemG.Size = New System.Drawing.Size(389, 279)
        Me.histogramaImagemG.TabIndex = 30
        Me.histogramaImagemG.Text = "Histograma"
        '
        'histogramaImagemB
        '
        Me.histogramaImagemB.BackColor = System.Drawing.Color.WhiteSmoke
        ChartArea6.Name = "ChartArea1"
        Me.histogramaImagemB.ChartAreas.Add(ChartArea6)
        Legend6.Enabled = False
        Legend6.Name = "Legend1"
        Me.histogramaImagemB.Legends.Add(Legend6)
        Me.histogramaImagemB.Location = New System.Drawing.Point(227, 315)
        Me.histogramaImagemB.Name = "histogramaImagemB"
        Series6.ChartArea = "ChartArea1"
        Series6.Legend = "Legend1"
        Series6.Name = "Series1"
        Me.histogramaImagemB.Series.Add(Series6)
        Me.histogramaImagemB.Size = New System.Drawing.Size(377, 279)
        Me.histogramaImagemB.TabIndex = 31
        Me.histogramaImagemB.Text = "Histograma"
        '
        'TelaVisualizacaoHistograma
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(874, 613)
        Me.Controls.Add(Me.histogramaImagemB)
        Me.Controls.Add(Me.histogramaImagemG)
        Me.Controls.Add(Me.histogramaImagemR)
        Me.Name = "TelaVisualizacaoHistograma"
        Me.Text = "Visualização de Histogramas"
        CType(Me.histogramaImagemR, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.histogramaImagemG, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.histogramaImagemB, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents histogramaImagemR As DataVisualization.Charting.Chart
    Friend WithEvents histogramaImagemG As DataVisualization.Charting.Chart
    Friend WithEvents histogramaImagemB As DataVisualization.Charting.Chart
End Class
