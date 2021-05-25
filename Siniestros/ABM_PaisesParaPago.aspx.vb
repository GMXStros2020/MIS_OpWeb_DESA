Imports System.Data
Imports Mensaje
Partial Class Siniestros_ABM_AnalistaSolicitante
    Inherits System.Web.UI.Page

    Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack Then

            CargarCombo()

            CargarGrid()

        End If

    End Sub





    Protected Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click



        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim iClave As Integer
        Dim oParametros As New Dictionary(Of String, Object)
        Try

            oParametros.Add("cod_pais", cmbPais.SelectedValue)
            If chkBanco.Checked Then
                oParametros.Add("sn_banco", -1)
            Else
                oParametros.Add("sn_banco", 0)
            End If
            If chkNumBanco.Checked Then
                oParametros.Add("sn_num_banco", -1)
            Else
                oParametros.Add("sn_num_banco", 0)
            End If

            If chkDomicilio.Checked Then
                oParametros.Add("sn_domicilio", -1)
            Else
                oParametros.Add("sn_domicilio", 0)
            End If

            If chkCuenta.Checked Then
                oParametros.Add("sn_cuenta", -1)
            Else
                oParametros.Add("sn_cuenta", 0)
            End If

            If chkAba.Checked Then
                oParametros.Add("sn_aba_routing", -1)
            Else
                oParametros.Add("sn_aba_routing", 0)
            End If

            If chkswift.Checked Then
                oParametros.Add("sn_swift", -1)
            Else
                oParametros.Add("sn_swift", 0)
            End If

            If chkTransit.Checked Then
                oParametros.Add("sn_transit", -1)
            Else
                oParametros.Add("sn_transit", 0)
            End If

            If chkIban.Checked Then
                oParametros.Add("sn_iban", -1)
            Else
                oParametros.Add("sn_iban", 0)
            End If


            oParametros.Add("Accion", 1)




            oDatos = Funciones.ObtenerDatos("sp_pais_para_pago_internacional", oParametros)
            CargarGrid()
            CargarCombo()




            Return

        Catch ex As Exception
            MuestraMensaje("Exception", "BuscarOP: " & ex.Message, TipoMsg.Falla)
            Return
        End Try



    End Sub

    Protected Sub btnStatus_Click(sender As Object, e As EventArgs) Handles btnStatus.Click
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim iClave As Integer

        For index = 0 To grd.Rows.Count - 1
            Dim sn_banco As CheckBox
            Dim sn_num_banco As CheckBox
            Dim sn_domicilio As CheckBox
            Dim sn_cuenta As CheckBox
            Dim sn_aba_routing As CheckBox
            Dim sn_swift As CheckBox
            Dim sn_transit As CheckBox
            Dim sn_iban As CheckBox
            Dim oParametros As New Dictionary(Of String, Object)
            Dim cod_pais As HiddenField

            cod_pais = grd.Rows(index).FindControl("cod_pais")

            sn_banco = grd.Rows(index).FindControl("sn_banco")
            sn_num_banco = grd.Rows(index).FindControl("sn_num_banco")
            sn_domicilio = grd.Rows(index).FindControl("sn_domicilio")
            sn_cuenta = grd.Rows(index).FindControl("sn_cuenta")
            sn_aba_routing = grd.Rows(index).FindControl("sn_aba_routing")
            sn_swift = grd.Rows(index).FindControl("sn_swift")
            sn_transit = grd.Rows(index).FindControl("sn_transit")
            sn_iban = grd.Rows(index).FindControl("sn_iban")



            oParametros.Add("cod_pais", cod_pais.Value)

            If sn_banco.Checked Then
                oParametros.Add("sn_banco", -1)
            Else
                oParametros.Add("sn_banco", 0)
            End If
            If sn_num_banco.Checked Then
                oParametros.Add("sn_num_banco", -1)
            Else
                oParametros.Add("sn_num_banco", 0)
            End If

            If sn_domicilio.Checked Then
                oParametros.Add("sn_domicilio", -1)
            Else
                oParametros.Add("sn_domicilio", 0)
            End If

            If sn_cuenta.Checked Then
                oParametros.Add("sn_cuenta", -1)
            Else
                oParametros.Add("sn_cuenta", 0)
            End If

            If sn_aba_routing.Checked Then
                oParametros.Add("sn_aba_routing", -1)
            Else
                oParametros.Add("sn_aba_routing", 0)
            End If

            If sn_swift.Checked Then
                oParametros.Add("sn_swift", -1)
            Else
                oParametros.Add("sn_swift", 0)
            End If

            If sn_transit.Checked Then
                oParametros.Add("sn_transit", -1)
            Else
                oParametros.Add("sn_transit", 0)
            End If

            If sn_iban.Checked Then
                oParametros.Add("sn_iban", -1)
            Else
                oParametros.Add("sn_iban", 0)
            End If


            oParametros.Add("Accion", 1)




            oDatos = Funciones.ObtenerDatos("sp_pais_para_pago_internacional", oParametros)


        Next

        Mensaje.MuestraMensaje("OrdenPagoSiniestros", "La informacion fue actualizada", TipoMsg.Advertencia)

    End Sub

    Private Sub CargarGrid()

        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        Dim Num_Lote As String
        Dim Fondos As String

        oParametros.Add("cod_pais", 2)
        oParametros.Add("Accion", 2)


        oDatos = Funciones.ObtenerDatos("sp_pais_para_pago_internacional", oParametros)


        oTabla = oDatos.Tables(0)

        grd.DataSource = oTabla

        grd.DataBind()



    End Sub

    Private Sub CargarCombo()


        Dim oDatos As DataSet



        Dim oParametros As New Dictionary(Of String, Object)

        oParametros.Add("catalogo", "Pais")

        oDatos = Funciones.ObtenerDatos("usp_obtener_cat_direccion", oParametros)

        cmbPais.DataSource = oDatos.Tables(0)
        cmbPais.DataTextField = "txt_desc"
        cmbPais.DataValueField = "cod_pais"
        cmbPais.DataBind()

        chkBanco.Checked = False
        chkNumBanco.Checked = False
        chkDomicilio.Checked = False
        chkCuenta.Checked = False
        chkAba.Checked = False
        chkswift.Checked = False
        chkTransit.Checked = False
        chkIban.Checked = False

    End Sub
    Protected Sub grd_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles grd.RowDeleting 'FJCP 10290_CC 
        Dim oDatos As DataSet
        Dim oParametros As New Dictionary(Of String, Object)

        Try
            Dim row As GridViewRow = grd.Rows(e.RowIndex)
            Dim cod_pais As String
            cod_pais = row.Cells(0).Text

            oParametros.Add("cod_pais", 1)
            oParametros.Add("txt_pais", cod_pais)
            oParametros.Add("Accion", 3)

            oDatos = Funciones.ObtenerDatos("sp_pais_para_pago_internacional", oParametros)
            CargarCombo()
            CargarGrid()

        Catch ex As Exception
            MuestraMensaje("Exception", ex.Message, TipoMsg.Falla)
        End Try

    End Sub
End Class

