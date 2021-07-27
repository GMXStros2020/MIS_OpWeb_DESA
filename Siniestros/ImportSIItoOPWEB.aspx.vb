Imports Mensaje
Imports System.Data
Imports Funciones

Partial Class ImportSIItoOPWEB

    Inherits System.Web.UI.Page

    Private Sub ImportSIItoOPWEB_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then

            CargarAnalistasFondos()

        End If
    End Sub

    Private Sub CargaAnalistas()
        Dim dtAnalistas As New DataTable

        fn_Consulta("mis_ObtieneAnalistasStro", dtAnalistas)
        LlenaDDL(ddlAnalista, dtAnalistas)


    End Sub

    Public Sub CargarAnalistasFondos()
        Try
            Dim oDatos As DataSet
            oDatos = New DataSet
            Dim oParametros As New Dictionary(Of String, Object)
            oParametros.Add("Accion", "3")
            oDatos = Funciones.ObtenerDatos("MIS_Catalago_Fondos", oParametros)
            Me.ddlAnalista.Items.Clear()
            If (oDatos.Tables(0).Rows.Count > 0) Then
                'With oDatos.Tables(0).Rows(0)
                '    Me.txtCodigoCuenta.Text = .Item("cod_cta_cble")
                '    Me.txtDescCuenta.Text = .Item("txt_denomin")
                'End With
                For Each fila In oDatos.Tables(0).Rows
                    Me.ddlAnalista.Items.Add(New ListItem(String.Format(fila.Item("AnalistaFondos")).ToUpper, fila.Item("usuarioanalista")))
                Next

            Else
                Mensaje.MuestraMensaje("Analistas de Fondos", "No hay Analista de Fondos", 1)
            End If
        Catch ex As Exception
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", String.Format("grd_RowDataBound error: {0}", ex.Message), 1)
        End Try
    End Sub

    Private Sub ddl_Modulo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddl_Modulo.SelectedIndexChanged
        If ddl_Modulo.SelectedItem.Value = 3 Then
            ddlAnalista.Enabled = True

        Else
            ddlAnalista.Enabled = False

        End If
    End Sub

    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Dim Parametros As String
        Dim Parametros2 As String
        Dim intResultado As Integer
        Dim intPrevio As Integer

        Try

            If ddl_Modulo.SelectedItem.Value = 0 Then
                Mensaje.MuestraMensaje("Importación de OPS a OPWEB", "Debe seleccionar un tipo de módulo", TipoMsg.Advertencia)
                Exit Sub
            End If

            Parametros = ddl_Modulo.SelectedValue & "," & txt_nro_op.Text & "," & txt_FolioOnBase.Text & "," & txt_nro_stro.Text &
                   "," & IIf(txt_nro_sol.Text = "", 0, txt_nro_sol.Text) & "," & txt_NoPago.Text


            If ddl_Modulo.SelectedItem.Value = 3 Then
                Parametros = Parametros & ",'" & ddlAnalista.SelectedItem.Value & "'"
            End If

            Parametros2 = ddl_Modulo.SelectedValue & "," & txt_nro_op.Text & "," & txt_FolioOnBase.Text & "," & txt_nro_stro.Text & "," & txt_NoPago.Text

            'busqueda para validar
            intPrevio = fn_Ejecuta("mis_BuscaCondiciones " & Parametros2)

            If intPrevio = 1 Then

                intResultado = fn_Ejecuta("sp_SIItoOPWEB " & Parametros)
                Mensaje.MuestraMensaje("Importación de OPS a OPWEB", "Se realizó la importación correctamente", TipoMsg.Confirma)
                btnLimpiar_Click(Nothing, Nothing)
            Else

                Mensaje.MuestraMensaje("Importación de OPS a OPWEB", "No existe registro con esas condiciones en la tabla MIS", TipoMsg.Falla)
            End If


        Catch ex As Exception
            Mensaje.MuestraMensaje("Importacion OPs", String.Format("import error:  {0}", ex.Message), TipoMsg.Falla)
        End Try



    End Sub

    Private Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        txt_FolioOnBase.Text = ""
        txt_nro_op.Text = ""
        txt_nro_sol.Text = ""
        txt_nro_stro.Text = ""
        txt_NoPago.Text = ""
        ddl_Modulo.SelectedIndex = 0
        ddlAnalista.SelectedIndex = 0
        ddlAnalista.Enabled = False

    End Sub
End Class
