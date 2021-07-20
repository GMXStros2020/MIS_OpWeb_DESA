Imports System.Data
Imports Mensaje
Partial Class Siniestros_ABM_AnalistaSolicitante
    Inherits System.Web.UI.Page

    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then


            Dim oDatos As DataSet



            Dim oParametros As New Dictionary(Of String, Object)

            oParametros.Add("Accion", 2)
            oParametros.Add("Folio_OnBase", "1")

            oDatos = Funciones.ObtenerDatos("MIS_sp_cir_op_stro_Catalogos_Fondos", oParametros)

            cmbOrigen.DataSource = oDatos.Tables(2)
            cmbOrigen.DataTextField = "DescripcionOrigenPago"
            cmbOrigen.DataValueField = "CodigoOrigenPago"
            cmbOrigen.DataBind()


            CargarGrid()

        End If

    End Sub


    Protected Sub btnConsulta_Click(sender As Object, e As EventArgs) Handles btnConsulta.Click
        Dim oDatos As DataSet



        Dim oParametros As New Dictionary(Of String, Object)
        Dim DT As New DataTable

        oParametros.Add("Accion", 2)
        oParametros.Add("cod_usuario", txtCodigoUsuario.Text)

        oDatos = Funciones.ObtenerDatos("MIS_Catalago_Fondos", oParametros)
        DT = oDatos.Tables(0)
        For Each row As DataRow In DT.Rows
            txtNombre.Text = row("AnalistaFondos").ToString()
        Next

        If DT.Rows.Count = 0 Then
            txtNombre.Text = ""
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Usuario no encontrado", TipoMsg.Advertencia)
        End If


    End Sub


    Protected Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click


        Dim analista As New AnalistaSolicitante

        analista.UsuarioSolicitante = txtCodigoUsuario.Text
        analista.Cod_origen_pago = cmbOrigen.SelectedValue
        analista.Sn_activo = -1
        analista.Save()


        txtCodigoUsuario.Text = ""
        txtNombre.Text = ""
        cmbOrigen.SelectedIndex = 0

        CargarGrid()

    End Sub

    Protected Sub btnStatus_Click(sender As Object, e As EventArgs) Handles btnStatus.Click
        Dim analista As New AnalistaSolicitante
        For index = 0 To grd.Rows.Count - 1
            Dim chk As CheckBox
            Dim txt As HiddenField
            chk = grd.Rows(index).FindControl("CheckBox1")
            txt = grd.Rows(index).FindControl("cod_origen_pago")
            If chk.Checked Then
                analista.Sn_activo = "-1"
                analista.UsuarioSolicitante = grd.Rows(index).Cells(0).Text
                analista.Cod_origen_pago = txt.Value
                analista.Save()

            Else
                analista.Sn_activo = "0"
                analista.UsuarioSolicitante = grd.Rows(index).Cells(0).Text
                analista.Cod_origen_pago = txt.Value
                analista.Save()
            End If
        Next

        Mensaje.MuestraMensaje("OrdenPagoSiniestros", "La informacion fue actualizada", TipoMsg.Advertencia)

    End Sub

    Private Sub CargarGrid()

        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim Num_Lote As String
        Dim Fondos As String


        oParametros.Add("Accion", 3)
        oParametros.Add("cod_usuario", txtCodigoUsuario.Text)

        oDatos = Funciones.ObtenerDatos("MIS_Catalago_Fondos", oParametros)


        oTabla = oDatos.Tables(0)

        grd.DataSource = oTabla

        grd.DataBind()



    End Sub
    Protected Sub grd_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles grd.RowDeleting

        Dim dt As New DataTable
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim iClave As Integer
        Dim oParametros As New Dictionary(Of String, Object)

        Try
            Dim row As GridViewRow = grd.Rows(e.RowIndex)
            Dim Usuario As String
            Dim OrigenPago As String
            Usuario = row.Cells(0).Text
            OrigenPago = row.Cells(2).Text


            oParametros.Add("UsuarioSolicitante", Usuario)
            oParametros.Add("cod_origen_pago", "0")
            oParametros.Add("sn_activo", "-1")
            oParametros.Add("Accion", "2")
            oParametros.Add("origen_pago", OrigenPago)

            oDatos = Funciones.ObtenerDatos("sp_grabar_analista_solicitante", oParametros)
            CargarGrid()


        Catch ex As Exception
            MuestraMensaje("Exception", ex.Message, TipoMsg.Falla)
        End Try
    End Sub

End Class
