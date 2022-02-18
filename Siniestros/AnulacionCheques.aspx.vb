
Imports System.Data
Imports System.Data.SqlClient
Imports Mensaje

Partial Class AnulacionCheques
    Inherits System.Web.UI.Page

    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then


            Dim oDatos As DataSet



            Dim oParametros As New Dictionary(Of String, Object)

            oParametros.Add("Accion", 4)


            oDatos = Funciones.ObtenerDatos("sp_catalogos_anulacion", oParametros)

            ddl_sucursal.DataSource = oDatos.Tables(0)
            ddl_sucursal.DataTextField = "txt_nom_suc"
            ddl_sucursal.DataValueField = "cod_suc"
            ddl_sucursal.DataBind()




        End If

    End Sub

    Protected Sub btnConsulta_Click(sender As Object, e As EventArgs) Handles btnConsulta.Click
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)


        oParametros.Add("Accion", 5)
        oParametros.Add("cod_usuario", Master.cod_usuario)

        If txt_cuenta_bancaria.Text.Length <> 0 Then
            oParametros.Add("banco", txt_cuenta_bancaria.Text)
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
                id_banco.Value = grd.Rows(index).Cells(4).Text
                cod_banco.Value = grd.Rows(index).Cells(3).Text
                txt_cuenta_bancaria.Text = grd.Rows(index).Cells(1).Text + "-" + grd.Rows(index).Cells(2).Text


            End If
        Next

        Funciones.EjecutaFuncion("$(""#Modal"").modal(""hide""); ")



    End Sub
    Protected Sub btnCheque_Click(sender As Object, e As EventArgs) Handles btnCheque.Click


        If String.IsNullOrEmpty(txt_nro_cheque.Text) = False Then


            Dim oDatos As DataSet
            Dim oTabla As DataTable
            Dim oParametros As New Dictionary(Of String, Object)


            oParametros.Add("Accion", 6)
            oParametros.Add("id_bco", id_banco.Value)
            oParametros.Add("nro_ch", txt_nro_cheque.Text)
            oParametros.Add("cod_suc_che", ddl_sucursal.SelectedValue)



            oDatos = Funciones.ObtenerDatos("sp_catalogos_anulacion", oParametros)
            If oDatos.Tables(0).Rows.Count = 0 Then
                Mensaje.MuestraMensaje("Anulación de Cheques", "Los datos ingresados no corresponden al Nro de cheque.", TipoMsg.Advertencia)
            Else
                oTabla = oDatos.Tables(0)
                For Each row As DataRow In oTabla.Rows
                    txt_op.Text = row("nro_op").ToString()
                    txt_fecha.Text = row("fec_pago").ToString()
                    txt_importe.Text = row("imp_ch").ToString()
                    txt_cheque_a_nombre.Text = row("txt_cheque_a_nom").ToString()
                Next
            End If
        Else
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Favor de Indicar el numero de cheque", TipoMsg.Advertencia)

        End If



    End Sub


    Protected Sub btn_AnularOP_Click(sender As Object, e As EventArgs) Handles btn_AnularOP.Click





        AnulaTransferencias()








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
        asiento = ""
        msg_err = ""

        oParametros.Add("nroOP", txt_op.Text)
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

        If msg_err.Length > 0 Then

            Mensaje.MuestraMensaje("OrdenPagoSiniestros", msg_err, TipoMsg.Advertencia)
        Else

            Mensaje.MuestraMensaje("OrdenPagoSiniestros", " Nro Cheque: " + txt_nro_cheque.Text + "\n Numero Transaccion: " + transaccion + "\n Asiento: " + asiento, TipoMsg.Advertencia)
        End If

        txt_op.Text = ""
        txt_fecha.Text = ""
        txt_importe.Text = ""
        txt_cheque_a_nombre.Text = ""
        txt_nro_cheque.Text = ""
    End Sub

End Class
