<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TelaVisualizacaoImagens
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
        Me.frameDemonstracao = New System.Windows.Forms.PictureBox()
        Me.save = New System.Windows.Forms.Button()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        CType(Me.frameDemonstracao, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'frameDemonstracao
        '
        Me.frameDemonstracao.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.frameDemonstracao.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.frameDemonstracao.Location = New System.Drawing.Point(0, 0)
        Me.frameDemonstracao.Name = "frameDemonstracao"
        Me.frameDemonstracao.Size = New System.Drawing.Size(799, 491)
        Me.frameDemonstracao.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.frameDemonstracao.TabIndex = 0
        Me.frameDemonstracao.TabStop = False
        '
        'save
        '
        Me.save.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.save.Location = New System.Drawing.Point(0, 491)
        Me.save.Name = "save"
        Me.save.Size = New System.Drawing.Size(799, 58)
        Me.save.TabIndex = 1
        Me.save.Text = "Salvar Imagem"
        Me.save.UseVisualStyleBackColor = True
        '
        'TelaVisualizacaoImagens
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(799, 491)
        Me.Controls.Add(Me.save)
        Me.Controls.Add(Me.frameDemonstracao)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "TelaVisualizacaoImagens"
        Me.Text = "Visualização de histórico"
        CType(Me.frameDemonstracao, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents frameDemonstracao As PictureBox
    Friend WithEvents save As Button
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
End Class
