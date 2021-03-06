﻿Imports Backend

Public Class PulidaForm
    Dim producc As New CargaProduccion
    Dim productoP As New Producto
    Public mesa As Integer
    Private Sub PulidaForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dgvPulida.DataSource = New DataSetProduccion.PulidaDataTable
        txtPulidora.Text = "PULIDORA " + mesa.ToString
        Dim prodDao As New ProduccionDAO
        lblPulidas.Text = prodDao.getPulidasDia()
        lblRoturas.Text = prodDao.getPulidasRoturasDia()
    End Sub

    Private Sub txtNroProd_KeyDown(sender As Object, e As KeyEventArgs) Handles txtNroProd.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Dim prodDao As New ProduccionDAO
            If txtNroProd.Text <> "" Then
                producc = prodDao.getProduccion(txtNroProd.Text)

                If producc.idProd <> 0 Then

                    If prodDao.validarCorte(producc.idProd) = False Then
                        MsgBox("Producción aun no pasó por corte", MsgBoxStyle.Critical, "Templado")
                        Exit Sub
                    End If
                    ' Guardar Corte
                    prodDao.guardarPulida(producc.idProd, mesa, 0)
                    'MsgBox("Pulida Guardada", MsgBoxStyle.Information, "Pulida")
                    dgvPulida.DataSource = prodDao.getPulida(producc.idProd).Tables("tabla")
                    txtNroProd.Text = ""
                    txtNroProd.Focus()
                    'Dim prodDao As New ProduccionDAO
                    lblPulidas.Text = prodDao.getPulidasDia()
                    lblRoturas.Text = prodDao.getPulidasRoturasDia()
                End If
            End If
        End If
    End Sub

    Private Sub btnMesa1_Click(sender As Object, e As EventArgs) Handles btnMesa1.Click
        Dim corteRot As New RoturaPulida
        corteRot.pulidora = mesa
        corteRot.ShowDialog()

        corteRot.Dispose()
        Dim prodDao As New ProduccionDAO
        lblPulidas.Text = prodDao.getPulidasDia()
        lblRoturas.Text = prodDao.getPulidasRoturasDia()
    End Sub
End Class