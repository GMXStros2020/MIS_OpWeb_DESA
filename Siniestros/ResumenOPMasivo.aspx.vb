
Imports System.Data

Partial Class Siniestros_ResumenOPMasivo
    Inherits System.Web.UI.Page
    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim Num_Lote As String

        Num_Lote = Request.QueryString("Num_Lote")

        If Num_Lote <> Nothing Then

            oParametros.Add("Num_Lote", Num_Lote)
            oParametros.Add("salida", 1)
            oDatos = Funciones.ObtenerDatos("sp_recupera_lote_OP_Masivo", oParametros)

            oTabla = oDatos.Tables(0)

            GridView1.DataSource = oTabla

            GridView1.DataBind()

        End If



    End Sub

    Protected Sub grd_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim fondos As String
        Dim PagarA As Integer

        fondos = Request.QueryString("Fondos")
        PagarA = Request.QueryString("PagarA")

        If fondos = 1 And PagarA = 10 Then
            If e.Row.RowType = DataControlRowType.Header Then
                e.Row.Cells(3).Visible = True
                e.Row.Cells(6).Visible = True
            End If

            If e.Row.RowType = DataControlRowType.DataRow Then
                e.Row.Cells(3).Visible = True
                e.Row.Cells(6).Visible = True
            End If
        Else
            If e.Row.RowType = DataControlRowType.Header Then
                e.Row.Cells(3).Visible = False
                e.Row.Cells(6).Visible = False
            End If

            If e.Row.RowType = DataControlRowType.DataRow Then
                e.Row.Cells(3).Visible = False
                e.Row.Cells(6).Visible = False
            End If
        End If

    End Sub

    Protected Sub grd_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView1.RowCommand
        Dim fondos As String
        Dim index As Integer
        Dim nro_op As String
        Dim nro_aut_tec As String

        fondos = Request.QueryString("Fondos")

        e.CommandArgument.ToString()
        index = CInt(e.CommandArgument.ToString())

        nro_op = GridView1.Rows(index).Cells(2).Text

        If e.CommandName = "RepCarta" Then
            generaReporte(nro_op)
            Exit Sub
        End If
        If e.CommandName = "RepCartaSolicitud" Then
            If fondos = "1" Then
                nro_aut_tec = GridView1.Rows(index).Cells(3).Text
                generaReporteSolicitud(nro_op, nro_aut_tec)
                Exit Sub
            End If
        End If
    End Sub


    Private Sub generaReporte(nrofolio As String)

        Dim ws As New ws_Generales.GeneralesClient
        Dim server As String = ws.ObtieneParametro(Cons.TargetReport)
        server = Replace(Replace(server, "@Reporte", "OrdenPago"), "@Formato", "PDF") & "&nro_op=" + nrofolio
        server = Replace(server, Cons.ReposSource, Cons.ReposReport)
        server = Replace(server, "OrdenPago", "OrdenPago_stro")
        'Funciones.EjecutaFuncion("fn_ImprimirOrden('" & server & "','" & "234777" & "');")
        'Funciones.EjecutaFuncion(String.Format("fn_ImprimirOrden('{0}','{1}');", server, nrofolio))

        Funciones.EjecutaFuncion("window.open('" & server & "');")
    End Sub



    Private Sub generaReporteSolicitud(nrofolio As String, nroauttec As String)
        'Impresion Solicitud de Pago

        Dim wssp As New ws_Generales.GeneralesClient
        Dim serversp As String = wssp.ObtieneParametro(Cons.TargetReport)
        'impresion de la solicitud de pago
        serversp = Replace(Replace(serversp, "@Reporte", "OrdenPago"), "@Formato", "PDF") & "&P_varios_op=" + nroauttec
        serversp = Replace(serversp, Cons.ReposSource, Cons.ReposReport)
        serversp = Replace(serversp, "OrdenPago", "SolicitudPago")
        'Funciones.EjecutaFuncion(String.Format("fn_ImprimirOrden('{0}','{1}');", serversp, oDatos.Tables(oDatos.Tables.Count - 1).Rows(0).Item("SolicitudPago")), "sp")
        Funciones.EjecutaFuncion("window.open('" & serversp & "');")
    End Sub

    Private Sub btnExportar_Click(sender As Object, e As EventArgs) Handles btnExportar.Click
        Dim ws As New ws_Generales.GeneralesClient
        Dim server As String = ws.ObtieneParametro(Cons.TargetReport)
        Dim Random As New Random()
        Dim numero As Integer = Random.Next(1, 1000)
        Dim Num_Lote As String
        Num_Lote = Request.QueryString("Num_Lote")
        Dim RptFilters As String
        RptFilters = "&NumLote=" & Num_Lote
        server = Replace(Replace(server, "@Reporte", "ResumenOP"), "@Formato", "EXCEL")
        server = Replace(server, Cons.ReposSource, Cons.ReposReport)
        server = server & RptFilters
        Funciones.EjecutaFuncion("window.open('" & server & "');")

    End Sub


End Class
