Imports System.Data
Imports Mensaje
Partial Class Siniestros_ReporteListadoPagosFondos
    Inherits System.Web.UI.Page

#Region "Eventos"
    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CargarCombos()
        End If
    End Sub

    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        txt_fec_gen_desde.Text = ""
        txt_fec_gen_hasta.Text = ""
        CargarCombos()
    End Sub

    Private Sub btnReporte_Click(sender As Object, e As EventArgs) Handles btnReporte.Click
        If ValidaFiltros() Then
            GeneraReporte()
            CargarCombos()
            MuestraMensaje("Listado de pago de fondos", "Reporte generado con éxito.", TipoMsg.Advertencia)
        End If
    End Sub
#End Region

#Region "Métodos"
    Private Sub CargarCombos()
        Dim oDatos As DataSet

        'Llena combos horas
        Dim oParametros As New Dictionary(Of String, Object)
        txt_fec_gen_desde.Text = ""
        txt_fec_gen_hasta.Text = ""

        oParametros.Add("strCatalogo", "catHoras")

        oDatos = Funciones.ObtenerDatos("sp_Catalogos_Repores_OPWEB", oParametros)

        ddlHoraDesde.DataSource = oDatos.Tables(0)
        ddlHoraDesde.DataValueField = "Codigo"
        ddlHoraDesde.DataTextField = "Hora"
        ddlHoraDesde.DataBind()

        ddlHoraHasta.DataSource = oDatos.Tables(0)
        ddlHoraHasta.DataValueField = "Codigo"
        ddlHoraHasta.DataTextField = "Hora"
        ddlHoraHasta.DataBind()

        'Llena combos minutos
        oDatos = New DataSet
        oParametros = New Dictionary(Of String, Object)

        oParametros.Add("strCatalogo", "catMinutos")

        oDatos = Funciones.ObtenerDatos("sp_Catalogos_Repores_OPWEB", oParametros)

        ddlMinutoDesde.DataSource = oDatos.Tables(0)
        ddlMinutoDesde.DataValueField = "Codigo"
        ddlMinutoDesde.DataTextField = "Minuto"
        ddlMinutoDesde.DataBind()

        ddlMinutoHasta.DataSource = oDatos.Tables(0)
        ddlMinutoHasta.DataValueField = "Codigo"
        ddlMinutoHasta.DataTextField = "Minuto"
        ddlMinutoHasta.DataBind()

        'Llena combo Fondos
        oDatos = New DataSet
        oParametros = New Dictionary(Of String, Object)

        oParametros.Add("strCatalogo", "origenPago")

        oDatos = Funciones.ObtenerDatos("sp_Catalogos_Repores_OPWEB", oParametros)

        ddlFondo.DataSource = oDatos.Tables(0)
        ddlFondo.DataValueField = "cod_origen_pago"
        ddlFondo.DataTextField = "descripcion"
        ddlFondo.DataBind()
    End Sub

    Private Sub GeneraReporte()
        Dim ws As New ws_Generales.GeneralesClient
        'Dim server As String = ws.ObtieneParametro(9)
        Dim server As String = ws.ObtieneParametro(3)
        Dim RptFilters As String
        RptFilters = "&Fondo=" & ddlFondo.SelectedValue.ToString()
        RptFilters = RptFilters & "&Fec_auto_ini=" & Funciones.FormatearFecha(txt_fec_gen_desde.Text, Funciones.enumFormatoFecha.YYYYMMDD) +
                     " " + ddlHoraDesde.SelectedItem.ToString() + ":" + ddlMinutoDesde.SelectedItem.ToString()
        RptFilters = RptFilters & "&Fec_auto_fin=" & Funciones.FormatearFecha(txt_fec_gen_hasta.Text, Funciones.enumFormatoFecha.YYYYMMDD) +
                     " " + ddlHoraHasta.SelectedItem.ToString() + ":" + ddlMinutoHasta.SelectedItem.ToString()

        server = Replace(Replace(server, "@Reporte", "Rpt_op_web_pagos_fondos"), "@Formato", "EXCEL")
        'server = Replace(server, "ReportesGMX_UAT", "ReportesOPSiniestros")
        server = Replace(server, "ReportesGMX_DESA", "ReportesOPSiniestros_DESA")
        server = server & RptFilters
        Funciones.EjecutaFuncion("window.open('" & server & "');")
    End Sub
#End Region

#Region "Funciones"
    Private Function ValidaFiltros() As Boolean
        Dim fec_desde As Date
        Dim fec_hasta As Date

        ValidaFiltros = True

        If Trim(txt_fec_gen_desde.Text) = "" Or Trim(txt_fec_gen_hasta.Text) = "" Then
            If Trim(txt_fec_gen_hasta.Text) = "" Then
                MuestraMensaje("Validación", "Debe elegir un rango de fechas para la consulta", TipoMsg.Advertencia)
                ValidaFiltros = False
            Else
                MuestraMensaje("Validación", "Debe seleccionar fecha autorización desde", TipoMsg.Advertencia)
                ValidaFiltros = False
            End If
        Else
            If Trim(txt_fec_gen_hasta.Text) <> "" Then
                fec_desde = txt_fec_gen_desde.Text
                fec_hasta = txt_fec_gen_hasta.Text

                If fec_desde > fec_hasta Then
                    MuestraMensaje("Validación", "Fecha autorización desde debe ser menor que fecha autorización hasta", TipoMsg.Advertencia)
                    ValidaFiltros = False
                End If

                If fec_desde = fec_hasta Then
                    If CDbl(ddlHoraDesde.SelectedValue) > CDbl(ddlHoraHasta.SelectedValue) Then
                        MuestraMensaje("Validación", "Hora desde debe ser menor que hora hasta", TipoMsg.Advertencia)
                        ValidaFiltros = False
                    End If

                    If CDbl(ddlHoraDesde.SelectedValue) = CDbl(ddlHoraHasta.SelectedValue) Then

                        If CDbl(ddlMinutoDesde.SelectedValue) > CDbl(ddlMinutoHasta.SelectedValue) Then
                            MuestraMensaje("Validación", "Minuto desde debe ser menor que minuto hasta", TipoMsg.Advertencia)
                            ValidaFiltros = False
                        End If
                    End If
                End If
            End If
        End If

        'If Trim(txt_fec_gen_desde.Text) <> "" And Trim(txt_fec_gen_hasta.Text) <> "" Then
        '    If Convert.ToDateTime(Me.txt_fec_gen_desde.Text) > Now.ToShortDateString Or Convert.ToDateTime(Me.txt_fec_gen_hasta.Text) > Now.ToShortDateString Then
        '        MuestraMensaje("Validación", "No Puede ingresar una fecha mayor al dia de hoy", TipoMsg.Advertencia)
        '        ValidaFiltros = False
        '    End If
        'End If
    End Function
#End Region
End Class
