Imports System.Data
Imports Mensaje
Partial Class Siniestros_ReporteAnalisisPagosStro
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
        'txt_ClaveAseg.Text = ""
        'txt_SearchAseg.Text = ""
        txt_benef.Text = ""
        txt_stro.Text = ""
        txt_poliza.Text = ""
        txt_ClaveProdu.Text = ""
        txt_SearchProd.Text = ""

        CargarCombos()
    End Sub

    Private Sub btnReporte_Click(sender As Object, e As EventArgs) Handles btnReporte.Click
        If ValidaFiltros() Then
            GeneraReporte()
            CargarCombos()
            MuestraMensaje("Reporte de análisis de pago de siniestros", "Reporte generado con éxito.", TipoMsg.Advertencia)
        End If
    End Sub
#End Region

#Region "Métodos"
    Private Sub CargarCombos()
        Dim oDatos As DataSet
        Dim dt As DataTable
        'Llena combos horas
        Dim oParametros As New Dictionary(Of String, Object)
        txt_fec_gen_desde.Text = ""
        txt_fec_gen_hasta.Text = ""
        'txt_ClaveAseg.Text = ""
        'txt_SearchAseg.Text = ""
        txt_benef.Text = ""
        txt_stro.Text = ""
        txt_poliza.Text = ""
        txt_ClaveProdu.Text = ""
        txt_SearchProd.Text = ""


        oParametros.Add("strCatalogo", "catAjustadores")

        oDatos = Funciones.ObtenerDatos("sp_Catalogos_Repores_OPWEB", oParametros)

        dt = Funciones.agregaTodos(oDatos.Tables(0))

        ddlAjustador.DataSource = dt
        ddlAjustador.DataValueField = "cod_pres"
        ddlAjustador.DataTextField = "descripcion"
        ddlAjustador.DataBind()

        'Llena combo solicitantes
        dt = New DataTable
        oParametros = New Dictionary(Of String, Object)

        oParametros.Add("strCatalogo", "catSolicitantes")

        oDatos = Funciones.ObtenerDatos("sp_Catalogos_Repores_OPWEB", oParametros)

        dt = Funciones.agregaTodos(oDatos.Tables(0))

        ddlSolicitante.DataSource = dt
        ddlSolicitante.DataValueField = "cod_usuario"
        ddlSolicitante.DataTextField = "txt_nombre"
        ddlSolicitante.DataBind()

        'Llena combo autorizadores
        dt = New DataTable
        oParametros = New Dictionary(Of String, Object)

        oParametros.Add("strCatalogo", "catAutorizadores")

        oDatos = Funciones.ObtenerDatos("sp_Catalogos_Repores_OPWEB", oParametros)

        dt = Funciones.agregaTodos(oDatos.Tables(0))

        ddlAutorizador.DataSource = dt
        ddlAutorizador.DataValueField = "cod_usuario"
        ddlAutorizador.DataTextField = "txt_nombre"
        ddlAutorizador.DataBind()
    End Sub

    Private Sub GeneraReporte()
        Dim ws As New ws_Generales.GeneralesClient
        'Dim server As String = ws.ObtieneParametro(9)
        Dim server As String = ws.ObtieneParametro(3)
        Dim RptFilters As String
        RptFilters = "&Fec_gene_ini=" & Funciones.FormatearFecha(txt_fec_gen_desde.Text, Funciones.enumFormatoFecha.YYYYMMDD)
        RptFilters = RptFilters & "&Fec_gene_fin=" & Funciones.FormatearFecha(txt_fec_gen_hasta.Text, Funciones.enumFormatoFecha.YYYYMMDD)

        'RptFilters = RptFilters & "&Beneficiario=" & IIf(txt_benef.Text.ToString() = "", "NULL", txt_benef.Text.ToString())
        'RptFilters = RptFilters & "&Siniestro=" & IIf(txt_stro.Text.ToString() = "", "NULL", txt_stro.Text.ToString())
        'RptFilters = RptFilters & "&Poliza=" & IIf(txt_poliza.Text.ToString() = "", "NULL", txt_poliza.Text.ToString())
        'RptFilters = RptFilters & "&Agente=" & IIf(txt_ClaveProdu.Text.ToString() = "", "NULL", txt_ClaveProdu.Text.ToString())

        If txt_benef.Text.ToString() <> "" Then
            RptFilters = RptFilters & "&Beneficiario=" & txt_benef.Text.ToString()
        End If

        If txt_stro.Text.ToString() <> "" Then
            RptFilters = RptFilters & "&Siniestro=" & txt_stro.Text.ToString()
        End If

        If txt_poliza.Text.ToString() <> "" Then
            RptFilters = RptFilters & "&Poliza=" & txt_poliza.Text.ToString()
        End If

        If txt_ClaveProdu.Text.ToString() <> "" Then
            RptFilters = RptFilters & "&Agente=" & txt_ClaveProdu.Text.ToString()
        End If

        RptFilters = RptFilters & "&Ajustador=" & ddlAjustador.SelectedValue.ToString()
        RptFilters = RptFilters & "&Solicitante=" & ddlSolicitante.SelectedValue.ToString()
        RptFilters = RptFilters & "&Usuario_aut=" & ddlAutorizador.SelectedValue.ToString()

        server = Replace(Replace(server, "@Reporte", "Rpt_op_web_analisis_pagos_stro"), "@Formato", "EXCEL")
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
