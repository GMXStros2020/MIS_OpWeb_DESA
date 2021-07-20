
Imports System.Data
Imports Mensaje

Partial Class Siniestros_CatalogodeDependencias
    Inherits System.Web.UI.Page

    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            CargarCombo()
            CargarGrid()
        End If

    End Sub

    Protected Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click

        Dim oDatos As DataSet

        Dim oParametros As New Dictionary(Of String, Object)
        Try
            If ValidaDatos() Then
                oParametros.Add("Accion", "1")
                oParametros.Add("PagarA", cmbTipoUsuario.SelectedValue)
                oParametros.Add("codigo", txtCodigoUsuario.Text)
                oParametros.Add("banco", ddl_banco.SelectedValue)
                oParametros.Add("sucursal", txt_sucursal.Text)

                oParametros.Add("plaza", txt_planza.Text)
                oParametros.Add("clabe", txt_clabe.Text)
                oParametros.Add("dependencia", txt_dependencia.Text.Trim())
                oParametros.Add("clave_referencia", txt_clave_referencia.Text)
                oParametros.Add("clave_dependencia", txt_clave_dependencia.Text)
                oParametros.Add("status", ddl_estatus.SelectedValue)

                oDatos = Funciones.ObtenerDatos("sp_grabar_dependencias_gob", oParametros)
                CargarGrid()
                CargarCombo()
                Return
            End If
        Catch ex As Exception
            MuestraMensaje("Exception", "BuscarOP: " & ex.Message, TipoMsg.Falla)
            Return
        End Try
    End Sub


    Protected Sub btnConsulta_Click(sender As Object, e As EventArgs) Handles btnConsulta.Click
        Dim oDatos As DataSet

        ddl_banco.SelectedIndex = 0
        txtNombre.Text = ""
        txt_sucursal.Text = ""
        txt_planza.Text = ""
        txt_clabe.Text = ""
        txt_clabe.Text = ""
        txt_dependencia.Text = ""
        txt_clave_referencia.Text = ""
        txt_clave_dependencia.Text = ""
        ddl_estatus.SelectedIndex = 1

        Dim oParametros As New Dictionary(Of String, Object)
        Dim DT As New DataTable

        oParametros.Add("Accion", 1)
        oParametros.Add("PagaA", cmbTipoUsuario.SelectedValue)
        oParametros.Add("codigo", txtCodigoUsuario.Text)

        oDatos = Funciones.ObtenerDatos("sp_consulta_dependendcias", oParametros)
        DT = oDatos.Tables(0)
        For Each row As DataRow In DT.Rows
            txtNombre.Text = row("nombre").ToString()
            If String.IsNullOrEmpty(row("cod_banco").ToString()) = False Then
                ddl_banco.SelectedValue = row("cod_banco").ToString()
            End If
            If String.IsNullOrEmpty(row("sucursal").ToString()) = False Then
                txt_sucursal.Text = row("sucursal").ToString()
            End If
            If String.IsNullOrEmpty(row("plaza").ToString()) = False Then
                txt_planza.Text = row("plaza").ToString()
            End If

            If String.IsNullOrEmpty(row("clabe").ToString()) = False Then
                txt_clabe.Text = row("clabe").ToString()
            End If

            If String.IsNullOrEmpty(row("dependencia").ToString()) = False Then
                txt_dependencia.Text = row("dependencia").ToString()
            End If

            If String.IsNullOrEmpty(row("clave_referencia").ToString()) = False Then
                txt_clave_referencia.Text = row("clave_referencia").ToString()
            End If

            If String.IsNullOrEmpty(row("clave_dependencia").ToString()) = False Then
                txt_clave_dependencia.Text = row("clave_dependencia").ToString()
            End If
            If String.IsNullOrEmpty(row("status").ToString()) = False Then
                ddl_estatus.SelectedValue = row("status").ToString()
            End If
        Next

        If DT.Rows.Count = 0 Then
            txtNombre.Text = ""
            Mensaje.MuestraMensaje("OrdenPagoSiniestros", "Usuario no encontrado", TipoMsg.Advertencia)
        End If
    End Sub

    Private Sub CargarCombo()
        Dim oDatos As DataSet
        Dim oParametros As New Dictionary(Of String, Object)

        oParametros.Add("Accion", 2)

        oDatos = Funciones.ObtenerDatos("sp_consulta_dependendcias", oParametros)

        ddl_banco.DataSource = oDatos.Tables(0)
        ddl_banco.DataTextField = "txt_nombre"
        ddl_banco.DataValueField = "cod_banco"
        ddl_banco.DataBind()
    End Sub

    Private Sub CargarGrid()

        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)

        oParametros.Add("Accion", 3)

        oDatos = Funciones.ObtenerDatos("sp_consulta_dependendcias", oParametros)

        oTabla = oDatos.Tables(0)

        grd.DataSource = oTabla

        grd.DataBind()

    End Sub

    Protected Sub grd_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles grd.RowDeleting 'FJCP 10290_CC 
        Dim idLbl As Label
        Try
            idLbl = grd.Rows(e.RowIndex).FindControl("id_dep")
            hid_id_dep.Value = idLbl.Text.Trim()
            Funciones.AbrirModal("#ModConfirmar")
        Catch ex As Exception
            MuestraMensaje("Exception", ex.Message, TipoMsg.Falla)
        End Try
    End Sub

    Public Sub grabar() 'FJCP 10290_CC 
        Dim oParametros As New Dictionary(Of String, Object)
        Dim id As Integer
        Dim oDatos As DataSet

        id = hid_id_dep.Value
        oParametros.Add("Accion", 4)
        oParametros.Add("id", id)

        oDatos = Funciones.ObtenerDatos("sp_consulta_dependendcias", oParametros)

        If oDatos.Tables(0).Rows(0).Item("err") <> 0 Then
            Funciones.CerrarModal("#ModConfirmar")
            MuestraMensaje("Eliminar Registro", oDatos.Tables(0).Rows(0).Item("err_desc").ToString, TipoMsg.Falla)
            CargarGrid()
        Else
            Funciones.CerrarModal("#ModConfirmar")
            'MuestraMensaje("Eliminar Registro", oDatos.Tables(0).Rows(0).Item("err_desc").ToString, TipoMsg.Advertencia)
            CargarGrid()
        End If
    End Sub

    Private Sub btnSi_Click(sender As Object, e As EventArgs) Handles btnSi.Click 'FJCP 10290_CC 
        grabar()
    End Sub

    Private Function ValidaDatos() As Boolean 'FJCP 10290_CC 
        Try
            If Trim(txt_clabe.Text) = "" Then
                ValidaDatos = False
                MuestraMensaje("Guardar", "Clabe es un campo obligatorio", TipoMsg.Falla)
            End If

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

End Class
