
Imports System.Data
Imports System.Data.SqlClient
Imports Mensaje

Partial Class Siniestros_AnulacionTranferencias
    Inherits System.Web.UI.Page


    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then


            Dim oDatos As DataSet



            Dim oParametros As New Dictionary(Of String, Object)

            oParametros.Add("Accion", 2)


            oDatos = Funciones.ObtenerDatos("sp_catalogos_anulacion", oParametros)

            ddl_concepto.DataSource = oDatos.Tables(0)
            ddl_concepto.DataTextField = "desc_concepto_anulacion"
            ddl_concepto.DataValueField = "cod_concepto_anulacion"
            ddl_concepto.DataBind()




        End If

    End Sub


    Protected Sub btnConsulta_Click(sender As Object, e As EventArgs) Handles btnConsulta.Click
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)


        oParametros.Add("Accion", 1)

        If txt_organismo.Text.Length <> 0 Then
            oParametros.Add("cod_org_financiero", txt_organismo.Text)
        End If

        oDatos = Funciones.ObtenerDatos("sp_catalogos_anulacion", oParametros)


        oTabla = oDatos.Tables(0)

        grd.DataSource = oTabla

        grd.DataBind()

        Funciones.EjecutaFuncion("$(""#Modal"").modal(""show""); ")



    End Sub

    Protected Sub btn_Aceptar_Click(sender As Object, e As EventArgs) Handles btn_Aceptar.Click


        For index = 0 To grd.Rows.Count - 1
            Dim chk As CheckBox

            chk = grd.Rows(index).FindControl("CheckBox1")

            If chk.Checked Then
                id_banco.Value = grd.Rows(index).Cells(1).Text
                txt_organismo.Text = grd.Rows(index).Cells(3).Text + "-" + grd.Rows(index).Cells(6).Text
                cod_organismo.Value = grd.Rows(index).Cells(2).Text

            End If
        Next

        Funciones.EjecutaFuncion("$(""#Modal"").modal(""hide""); ")



    End Sub

    Protected Sub btnOrden_Click(sender As Object, e As EventArgs) Handles btnOrden.Click
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)

        If id_banco.Value = "" Then
            Exit Sub
        End If

        oParametros.Add("Accion", 3)

        oParametros.Add("id_bco", id_banco.Value)
        If txt_orden_pago.Text.Length > 0 Then
            oParametros.Add("nro_op", txt_orden_pago.Text)
        End If

        oDatos = Funciones.ObtenerDatos("sp_catalogos_anulacion", oParametros)


        oTabla = oDatos.Tables(0)

        grdOp.DataSource = oTabla

        grdOp.DataBind()

        Funciones.EjecutaFuncion("$(""#ModalOp"").modal(""show""); ")



    End Sub

    Protected Sub btnAceptarOp_Click(sender As Object, e As EventArgs) Handles btnAceptarOp.Click


        For index = 0 To grdOp.Rows.Count - 1
            Dim chk As CheckBox

            chk = grdOp.Rows(index).FindControl("CheckBox2")

            If chk.Checked Then
                txt_orden_pago.Text = grdOp.Rows(index).Cells(1).Text
                txt_fecha.Text = grdOp.Rows(index).Cells(2).Text
                txt_importe.Text = grdOp.Rows(index).Cells(3).Text
                txt_tranferencia.Text = grdOp.Rows(index).Cells(4).Text
                txt_cuenta.Text = grdOp.Rows(index).Cells(6).Text
                txt_transaccion.Text = grdOp.Rows(index).Cells(5).Text

            End If
        Next

        Funciones.EjecutaFuncion("$(""#ModalOp"").modal(""hide""); ")



    End Sub

    Protected Function ValidaDatos() As Boolean

        ValidaDatos = True
        Dim sTexto As String
        sTexto = ""
        If txt_organismo.Text = "" Or Len(txt_organismo.Text) = 0 Then
            ValidaDatos = False
            sTexto += "Falta indicar el organismo financiero </br> "

        ElseIf Not IsNumeric(txt_orden_pago.Text) Then
            ValidaDatos = False
            sTexto += "Falta indicar la orden de pago a anular "

        ElseIf Not IsNumeric(txt_transaccion.Text) Then
            ValidaDatos = False
            sTexto += "La orden de pago indicada aún no ha sido contabilizada "

        ElseIf ddl_concepto.SelectedIndex < 0 Then
            ValidaDatos = False
            sTexto += "Falta indicar el concepto de anulación de la transferencia "

        End If
        If ValidaDatos = False Then
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", sTexto, TipoMsg.Advertencia)
        End If


    End Function

    Protected Sub btn_AnularOP_Click(sender As Object, e As EventArgs) Handles btn_AnularOP.Click


        Dim res As Boolean
        Dim sMensajeError As String


        If ValidaDatos() = True Then
            AnulaTransferencias()

        End If






    End Sub

    Protected Sub AnulaTransferencias()

        Dim oComando As SqlCommand
        Dim oParametros As New Dictionary(Of String, Object)
        Dim lstparametrosOut As New List(Of SqlParameter)
        Dim parametrosOut As New SqlParameter
        Dim ssql As String
        Dim transaccion As String
        Dim asiento As String
        Dim msg_err As String
        transaccion = ""
        oParametros.Add("cod_cpto_anulacion", ddl_concepto.SelectedValue)
        oParametros.Add("nroOP", txt_orden_pago.Text)
        oParametros.Add("codUsuario", Master.cod_usuario)
        oParametros.Add("sucUsuario", "1")

        parametrosOut.ParameterName = "@sn_flag"
        parametrosOut.SqlDbType = SqlDbType.Int
        parametrosOut.Size = 32
        parametrosOut.Direction = ParameterDirection.Output
        lstparametrosOut.Add(parametrosOut)

        parametrosOut = New SqlParameter
        parametrosOut.ParameterName = "@msg_err"
        parametrosOut.SqlDbType = SqlDbType.VarChar
        parametrosOut.Size = 300
        parametrosOut.Direction = ParameterDirection.Output
        lstparametrosOut.Add(parametrosOut)

        parametrosOut = New SqlParameter
        parametrosOut.ParameterName = "@nroTransaccion"
        parametrosOut.SqlDbType = SqlDbType.Int
        parametrosOut.Size = 32
        parametrosOut.Direction = ParameterDirection.Output
        lstparametrosOut.Add(parametrosOut)

        parametrosOut = New SqlParameter
        parametrosOut.ParameterName = "@nro_asiento"
        parametrosOut.SqlDbType = SqlDbType.Decimal
        parametrosOut.Size = 32
        parametrosOut.Direction = ParameterDirection.Output
        lstparametrosOut.Add(parametrosOut)

        oComando = Funciones.ObtenerDatosOut("usp_anula_trans_banc_cheques ", oParametros, lstparametrosOut)

        msg_err = oComando.Parameters("@msg_err").Value.ToString()
        transaccion = oComando.Parameters("@nroTransaccion").Value.ToString()
        asiento = oComando.Parameters("@nro_asiento").Value.ToString()
        '-----------------
        Dim Msg, Style, Title, Response, MyString
        Msg = " Orden de pago: " + txt_orden_pago.Text + vbCrLf + " Numero Transaccion: " + transaccion + vbCrLf + " Asiento: " + asiento    ' Define message.
        Style = vbOKOnly    ' Define buttons.
        Title = "OrdenPagoSiniestros"    ' Define title. 
        ' Display message.
        Response = MsgBox(Msg, Style, Title)
        Select Case Response
            Case vbOK
                txt_orden_pago.Text = ""
                txt_fecha.Text = ""
                txt_importe.Text = ""
                txt_tranferencia.Text = ""
                txt_cuenta.Text = ""
                txt_transaccion.Text = ""
                txt_organismo.Text = ""
        End Select
        'Mensaje.MuestraMensaje("OrdenPagoSiniestros", " Orden de pago: " + txt_orden_pago.Text + "\n Numero Transaccion: " + transaccion + "\n Asiento: " + asiento, TipoMsg.Advertencia)
        '-----------------
    End Sub

End Class
