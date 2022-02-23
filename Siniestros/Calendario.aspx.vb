
Imports System.Data

Partial Class Siniestros_Calendario
    Inherits System.Web.UI.Page


    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            CargarGrid()
        End If


    End Sub

    Protected Sub btn_Agregar_Click(sender As Object, e As EventArgs) Handles btn_Agregar.Click


        Dim oParametros As New Dictionary(Of String, Object)
        Dim FecHoy As String
        FecHoy = DateTime.Now.ToString("dd/MM/yyyy")
        Try
            oParametros = New Dictionary(Of String, Object)


            If txt_descripcion.Text = "" Then
                Mensaje.MuestraMensaje("Calendario", "Favor de caputurar una descripcion", Mensaje.TipoMsg.Advertencia)
                Exit Sub
            End If

            If txt_fecha_ini.Text = "" Then
                Mensaje.MuestraMensaje("Calendario", "Favor de seleccionar una fecha", Mensaje.TipoMsg.Advertencia)
                Exit Sub
            End If

            If (Convert.ToDateTime(txt_fecha_ini.Text) < FecHoy) Then
                Mensaje.MuestraMensaje("Calendario", "La fecha ingresada no corresponde, ya que el suceso ya ocurrió.", Mensaje.TipoMsg.Advertencia)
                Exit Sub
            End If

            'oParametros.Add("TipoMoneda", Me.cmbMonedaPago.SelectedValue)
            oParametros.Add("accion", 1)
            oParametros.Add("Descripcion", txt_descripcion.Text)
            oParametros.Add("fecha", Funciones.FormatearFecha(txt_fecha_ini.Text, Funciones.enumFormatoFecha.YYYYMMDD)
                                                             )


            Funciones.ObtenerDatos("sp_grabar_feriados_web", oParametros)


            txt_descripcion.Text = ""
            txt_fecha_ini.Text = ""

            CargarGrid()

        Catch ex As Exception

            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ObtenerTipoCambio error: {0}", ex.Message), Mensaje.TipoMsg.Advertencia)
        End Try
    End Sub


    Private Sub CargarGrid()
        Dim oParametros As New Dictionary(Of String, Object)
        Dim oDatos As DataSet
        Dim oTabla As DataTable

        oDatos = Funciones.ObtenerDatos("sp_consulta_feriados_web", oParametros)

        oTabla = oDatos.Tables(0)

        grd.DataSource = oTabla


        grd.DataBind()
    End Sub


    Protected Sub grd_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles grd.RowDeleting


        Dim oParametros As New Dictionary(Of String, Object)
        Dim txt_fecha_ini As Label
        txt_fecha_ini = grd.Rows(e.RowIndex).FindControl("fecha")

        Try
            oParametros = New Dictionary(Of String, Object)



            'oParametros.Add("TipoMoneda", Me.cmbMonedaPago.SelectedValue)
            oParametros.Add("accion", 2)
            oParametros.Add("fecha", Funciones.FormatearFecha(txt_fecha_ini.Text, Funciones.enumFormatoFecha.YYYYMMDD))



            Funciones.ObtenerDatos("sp_grabar_feriados_web", oParametros)


            txt_descripcion.Text = ""
            txt_fecha_ini.Text = ""

            CargarGrid()

        Catch ex As Exception

            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("ObtenerTipoCambio error: {0}", ex.Message), Mensaje.TipoMsg.Advertencia)
        End Try



    End Sub


    Protected Sub btn_Revisar_Click(sender As Object, e As EventArgs) Handles btn_Revisar.Click

        Dim ws As New ws_Generales.GeneralesClient
        Dim server As String = ws.ObtieneParametro(Cons.TargetReport)
        Dim RptFilters As String
        RptFilters = ""


        server = Replace(Replace(server, "@Reporte", "DiasFeriados"), "@Formato", "EXCEL")
        server = Replace(server, Cons.ReposSource, Cons.ReposReport)
        server = server & RptFilters
        Funciones.EjecutaFuncion("window.open('" & server & "');")





    End Sub
End Class
