Imports Microsoft.VisualBasic
Imports Mensaje
Imports System.Data

Public Class Tercero
    'Dim aDir() As String

    'Dim aTelefono() As String

    Dim _sTipoPersona As String
    Dim _iPersona As Long
    Dim _iCodigoTercero As Long
    Dim _iCodigoTerceroNvo As Long
    Dim _sApellido1 As String
    Dim _sApellido2 As String
    Dim _sNombre As String
    Dim _sNit As String
    Dim _sSexo As String
    Dim _iEstCivil As Integer
    Dim _vFecNacim As String
    Dim _sLugarNacim As String
    Dim _iEdad As Integer
    Dim _sOcupacion As String
    Dim _dSueldo As Double
    Dim _sParentesco As String
    Dim _sNombrePariente As String
    Dim _sNroSSPariente As String


    Dim _iTipoDir As Integer
    Dim _icodPais As Integer
    Dim _icodDpto As Integer
    Dim _icodMun As Integer
    Dim _scodPostal As String
    Dim _icodCalle1 As Integer
    Dim _sBarrio As String
    Dim _sNroCasa As String
    Dim _sNomCalle As String
    Dim _sOfic As String

    Dim _sEmail As String
    Dim _sTelCasa As String
    Dim _sTelTrab As String
    Dim _sCel As String



    Dim _iOcupacion As Integer
    Dim _sOrigen As String
    Dim _sFecUltModif As String
    Dim _sCodUsuario As String
    Dim _iCodParentesco As Integer

    Public Property sTipoPersona As String
        Get
            Return _sTipoPersona
        End Get
        Set(value As String)
            _sTipoPersona = value
        End Set
    End Property

    Public Property iPersona As Long
        Get
            Return _iPersona
        End Get
        Set(value As Long)
            _iPersona = value
        End Set
    End Property

    Public Property iCodigoTercero As Long
        Get
            Return _iCodigoTercero
        End Get
        Set(value As Long)
            _iCodigoTercero = value
        End Set
    End Property

    Public Property sApellido1 As String
        Get
            Return _sApellido1
        End Get
        Set(value As String)
            _sApellido1 = value
        End Set
    End Property

    Public Property sApellido2 As String
        Get
            Return _sApellido2
        End Get
        Set(value As String)
            _sApellido2 = value
        End Set
    End Property

    Public Property sNombre As String
        Get
            Return _sNombre
        End Get
        Set(value As String)
            _sNombre = value
        End Set
    End Property

    Public Property sNit As String
        Get
            Return _sNit
        End Get
        Set(value As String)
            _sNit = value
        End Set
    End Property

    Public Property sSexo As String
        Get
            Return _sSexo
        End Get
        Set(value As String)
            _sSexo = value
        End Set
    End Property

    Public Property iEstCivil As Integer
        Get
            Return _iEstCivil
        End Get
        Set(value As Integer)
            _iEstCivil = value
        End Set
    End Property

    Public Property vFecNacim As String
        Get
            Return _vFecNacim
        End Get
        Set(value As String)
            _vFecNacim = value
        End Set
    End Property

    Public Property sLugarNacim As String
        Get
            Return _sLugarNacim
        End Get
        Set(value As String)
            _sLugarNacim = value
        End Set
    End Property

    Public Property iEdad As Integer
        Get
            Return _iEdad
        End Get
        Set(value As Integer)
            _iEdad = value
        End Set
    End Property

    Public Property sOcupacion As String
        Get
            Return _sOcupacion
        End Get
        Set(value As String)
            _sOcupacion = value
        End Set
    End Property

    Public Property dSueldo As Double
        Get
            Return _dSueldo
        End Get
        Set(value As Double)
            _dSueldo = value
        End Set
    End Property

    Public Property sParentesco As String
        Get
            Return _sParentesco
        End Get
        Set(value As String)
            _sParentesco = value
        End Set
    End Property

    Public Property sNombrePariente As String
        Get
            Return _sNombrePariente
        End Get
        Set(value As String)
            _sNombrePariente = value
        End Set
    End Property

    Public Property sNroSSPariente As String
        Get
            Return _sNroSSPariente
        End Get
        Set(value As String)
            _sNroSSPariente = value
        End Set
    End Property

    Public Property sEmail As String
        Get
            Return _sEmail
        End Get
        Set(value As String)
            _sEmail = value
        End Set
    End Property

    Public Property iTipoDir As Integer
        Get
            Return _iTipoDir
        End Get
        Set(value As Integer)
            _iTipoDir = value
        End Set
    End Property

    Public Property icodPais As Integer
        Get
            Return _icodPais
        End Get
        Set(value As Integer)
            _icodPais = value
        End Set
    End Property

    Public Property icodDpto As Integer
        Get
            Return _icodDpto
        End Get
        Set(value As Integer)
            _icodDpto = value
        End Set
    End Property

    Public Property icodMun As Integer
        Get
            Return _icodMun
        End Get
        Set(value As Integer)
            _icodMun = value
        End Set
    End Property

    Public Property scodPostal As String
        Get
            Return _scodPostal
        End Get
        Set(value As String)
            _scodPostal = value
        End Set
    End Property

    Public Property icodCalle1 As Integer
        Get
            Return _icodCalle1
        End Get
        Set(value As Integer)
            _icodCalle1 = value
        End Set
    End Property

    Public Property sBarrio As String
        Get
            Return _sBarrio
        End Get
        Set(value As String)
            _sBarrio = value
        End Set
    End Property

    Public Property sNroCasa As String
        Get
            Return _sNroCasa
        End Get
        Set(value As String)
            _sNroCasa = value
        End Set
    End Property

    Public Property sNomCalle As String
        Get
            Return _sNomCalle
        End Get
        Set(value As String)
            _sNomCalle = value
        End Set
    End Property

    Public Property sOfic As String
        Get
            Return _sOfic
        End Get
        Set(value As String)
            _sOfic = value
        End Set
    End Property

    Public Property iOcupacion As Integer
        Get
            Return _iOcupacion
        End Get
        Set(value As Integer)
            _iOcupacion = value
        End Set
    End Property

    Public Property sOrigen As String
        Get
            Return _sOrigen
        End Get
        Set(value As String)
            _sOrigen = value
        End Set
    End Property

    Public Property sFecUltModif As String
        Get
            Return _sFecUltModif
        End Get
        Set(value As String)
            _sFecUltModif = value
        End Set
    End Property

    Public Property sCodUsuario As String
        Get
            Return _sCodUsuario
        End Get
        Set(value As String)
            _sCodUsuario = value
        End Set
    End Property

    Public Property iCodParentesco As Integer
        Get
            Return _iCodParentesco
        End Get
        Set(value As Integer)
            _iCodParentesco = value
        End Set
    End Property

    Public Property sTelCasa As String
        Get
            Return _sTelCasa
        End Get
        Set(value As String)
            _sTelCasa = value
        End Set
    End Property

    Public Property sTelTrab As String
        Get
            Return _sTelTrab
        End Get
        Set(value As String)
            _sTelTrab = value
        End Set
    End Property

    Public Property sCel As String
        Get
            Return _sCel
        End Get
        Set(value As String)
            _sCel = value
        End Set
    End Property

    Public Property ICodigoTerceroNvo As Long
        Get
            Return _iCodigoTerceroNvo
        End Get
        Set(value As Long)
            _iCodigoTerceroNvo = value
        End Set
    End Property


    'Public Property aDir As String()
    '    Get
    '        Return _aDir
    '    End Get
    '    Set(value As String())
    '        _aDir = value
    '    End Set
    'End Property

    'Public Property aTelefono As String()
    '    Get
    '        Return _aTelefono
    '    End Get
    '    Set(value As String())
    '        _aTelefono = value
    '    End Set
    'End Property




    Public Function RFC() As Boolean
        Dim pos As Integer
        Dim arrParamRFC() As String
        Dim cadena As String
        Dim iparam_rfc As String
        Dim iparam_rfc_otr_ter As String
        Dim oDatos As DataSet
        'Dim oParametros As New Dictionary(Of String, Object)
        'Dim txtnit As String
        Dim brfcGen As Boolean

        Try
            'txtnit = txt_rfc.Text.Trim

            cadena = ""
            iparam_rfc = ""
            iparam_rfc_otr_ter = ""

            'obtener rfc genericos
            oDatos = Funciones.ObtenerDatos("usp_obtener_rfc_genericos")

            If Not oDatos.Tables(0) Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                iparam_rfc = oDatos.Tables(0).Rows(0).Item("txt_param")
            End If

            If Not oDatos.Tables(1) Is Nothing AndAlso oDatos.Tables(1).Rows.Count > 0 Then
                iparam_rfc_otr_ter = oDatos.Tables(1).Rows(0).Item("txt_param")
            End If



            arrParamRFC = Split(iparam_rfc_otr_ter, "|", -1, 1)
            For pos = LBound(arrParamRFC) To UBound(arrParamRFC)
                If arrParamRFC(pos) <> sNit Then
                    If iparam_rfc <> sNit Then
                        If vFecNacim = "" Then
                            MuestraMensaje("Fecha", "FALTA FECHA DE NACIMIENTO/CONSTITUCION", TipoMsg.Falla)
                            Return False
                            'Exit Function
                        End If
                    End If
                End If
            Next


            For pos = LBound(arrParamRFC) To UBound(arrParamRFC)
                cadena = cadena & arrParamRFC(pos) & "','"
            Next

            cadena = "'" & cadena & iparam_rfc & "'"


            brfcGen = cadena.Contains(sNit)
            'VZAVALETA_GMX-10290_INCIDENCIAS
            If (Len(Trim(sNit)) < 12) Then
                MuestraMensaje("Valida RFC", "RFC INCORRECTO", TipoMsg.Falla)
                Return False
            End If
            'VZAVALETA_GMX-10290_INCIDENCIAS
            If brfcGen = False Then
                If ValidaRFC() = False Then
                    MuestraMensaje("Valida RFC", "RFC Ó FECHA NACIMIENTO/CONSTITUCIÓN INCORRECTO", TipoMsg.Falla)
                    'Exit Function
                    Return False
                End If
            End If


            Return True
        Catch ex As Exception
            MuestraMensaje("Exception", "BuscarOP: " & ex.Message, TipoMsg.Falla)
            Return False
        End Try
    End Function

    Private Function ValidaRFC() As Boolean
        'Habilitar el campo RFC tanto para personas físicas como morales,
        'siendo éste obligatorio en ambos casos.
        'Se debe incluir una validación para que:
        'Dim sApePaterno As String
        'Dim sApeMaterno As String
        'Dim sNombre As String
        Dim sFechaNac As String
        Dim vi As Integer
        'Dim txtnit As String
        Dim key As Integer

        'txtnit = Trim(txt_rfc.Text)
        'sApePaterno = Trim(txt_apPat.Text)
        'sApeMaterno = Trim(txt_apMat.Text)
        'sNombre = Trim(txt_nombres.Text)
        sFechaNac = vFecNacim

        ValidaRFC = True



        Try
            If sTipoPersona.ToString() = "F" Then
                'If Parametro <> txtnit Then   'CMM 7234 PARA QUE NO VALIDE RFC GENERICO
                Dim sRFC As String = CalculaRFC(sApellido1, sApellido2, sNombre, sFechaNac)
                'En personas Físicas tenga 13 caracteres
                If Not (Len(Trim(sNit)) = 13) Then
                    ValidaRFC = False
                    'y cuatro primeras letras de acuerdo a su apellido y nombre y posteriormente la fecha de nacimiento AAMMDD
                ElseIf Mid(sRFC, 1, 4) <> Mid(Trim(sNit), 1, 4) OrElse Mid(sRFC, 5, 6) <> Mid(Trim(sNit), 5, 6) Then
                    ValidaRFC = False
                Else
                    'n Posición 11 a 13 alfanumérico obligatorio
                    For vi = 11 To 13
                        key = Asc(Mid(sNit, vi, 1))
                        'If (Asc(Mid(txtnit, vi, 1)) >= 48 And Asc(Mid(txtnit, vi, 1)) <= 57) Or (Asc(Mid(txtnit, vi, 1)) >= 65 And Asc(Mid(txtnit, vi, 1) <= 90)) Then
                        If key >= 48 And key <= 57 OrElse key >= 65 And key <= 90 Then
                            ValidaRFC = True
                        Else
                            ValidaRFC = False
                            Exit For
                        End If
                    Next vi
                End If

            ElseIf sTipoPersona.ToString() = "J" Then
                'En personas Morales tenga 12 caracteres
                'COMO SE CALCULA LAS 3 PRIMERAS LETRAS????? y cuatro primeras letras de acuerdo al nombre de la compañía
                If Not (Len(Trim(sNit)) = 12) Then
                    ValidaRFC = False
                    'Posición 4 a 9 debe contener el año, mes y día de constitución de la empresa (AAMMDD)
                    'ElseIf Mid(sFechaNac, 3, 2) + Format(Mid(sFechaNac, 5, 2), "00") + Format(Mid(sFechaNac, 7, 2), "00") <> Mid(Trim(sNit), 4, 6) Then
                ElseIf Mid(sFechaNac, 3, 2) + Mid(sFechaNac, 5, 2) + Mid(sFechaNac, 7, 2) <> Mid(Trim(sNit), 4, 6) Then
                    ValidaRFC = False
                Else
                    For vi = 1 To 12
                        Select Case vi

                        'Posición 1 a 3 debe contener letras y simbolos Autorizados.
                            Case 1, 2, 3
                                key = Asc(Mid(sNit, vi, 1))
                                'If Not (Asc(Mid(sNit, vi, 1)) >= 65 Or Asc(Mid(sNit, vi, 1)) <= 90) _
                                '    And Asc(Mid(sNit, vi, 1)) <> 38 Then
                                If Not (key >= 65 Or key <= 90) AndAlso key <> 38 Then
                                    ValidaRFC = False
                                    Exit For
                                End If
                        'n Posición 10,11,12  alfanumérica.
                            Case 10, 11, 12
                                key = Asc(Mid(sNit, vi, 1))
                                'If (Asc(Mid(sNit, vi, 1)) >= 48 And Asc(Mid(sNit, vi, 1)) <= 57) Or (Asc(Mid(sNit, vi, 1)) >= 65 And Asc(Mid(sNit, vi, 1) <= 90)) Then
                                If key >= 48 And key <= 57 OrElse key >= 65 And key <= 90 Then
                                    ValidaRFC = True
                                Else
                                    ValidaRFC = False
                                    Exit For
                                End If
                        End Select
                    Next vi
                End If
            End If

        Catch ex As Exception
            MuestraMensaje("excepcion", ex.Message, TipoMsg.Advertencia)
        End Try
    End Function

    Private Function CalculaRFC(pApePaterno As String, pApeMaterno As String, pNombre As String, sFechaNacimiento As String) As String
        Dim Apellido As String
        Dim vs_1er_Valor As String
        Dim vs_2do_Valor As String
        Dim vStr_Compare As String
        Dim vi As Integer
        vStr_Compare = ""
        'Valida que el nombre compuesto no comience con Maria o Jose

        pNombre = fsDetectaNombre(pNombre)

        ' Regla 7a. La persona física cuenta nada más con un apellido en caso
        ' contrario realiza el proceso normal
        If pApePaterno = "" Or pApeMaterno = "" Then
            If pApePaterno <> "" Then
                Apellido = pApePaterno
            Else
                Apellido = pApeMaterno
            End If

            ' Regla 4a. En los casos en que el apellido de la persona física se
            ' componga de 1 o 2 letras, se debe tomar la 1era letra del apellido
            ' y 2da letra del nombre
            If Len(Apellido) > 2 Then
                For vi = 2 To Len(Apellido)
                    vStr_Compare = Mid(Apellido, vi, 1)
                    Select Case vStr_Compare
                        Case "A", "E", "I", "O", "U"
                            Exit For
                    End Select
                Next vi
                vs_1er_Valor = Left(Apellido, 1) + vStr_Compare + Left(pNombre, 2)
            Else
                vs_1er_Valor = Left(Apellido, 1) + Left(pNombre, 2)
            End If
        Else
            If Len(pApePaterno) > 2 Then
                For vi = 2 To Len(pApePaterno)
                    vStr_Compare = Mid(pApePaterno, vi, 1)
                    Select Case vStr_Compare
                        Case "A", "E", "I", "O", "U"
                            Exit For
                    End Select
                Next vi
                vs_1er_Valor = Left(pApePaterno, 1) + vStr_Compare + Left(pApeMaterno, 1) + Left(pNombre, 1)
            Else
                vs_1er_Valor = Left(pApePaterno, 1) + Left(pApeMaterno, 1) + Left(pNombre, 2)
            End If
        End If

        vs_2do_Valor = Mid(sFechaNacimiento, 3, 2) + Mid(sFechaNacimiento, 5, 2) + Mid(sFechaNacimiento, 7, 2)

        CalculaRFC = vs_1er_Valor + vs_2do_Valor
    End Function

    Private Function fsDetectaNombre(ByVal sNombre As String) As String
        '******************************************************************
        '** Funcion:  fsDetectaNombre                                    **
        '** Objetivo: Esta funcion verifica que el nombre, ingresado como**
        '**           parametro no contenga nombres compuestos que       **
        '**           comiencen con MARIA o JOSE                         **
        '**                                                              **
        '** Parametros de Entrada:                                       **
        '**         sNombre      - Nombre                                **
        '**                                                              **
        '** Parametros de Salida:                                        **
        '**         Regresa el nombre sin los prefijos de MARIA o JOSE   **
        '******************************************************************
        Dim iPos As Integer

        ' Excluimos las preposiciones del NOmbre
        sNombre = fsExtraePreposicion(sNombre)

        ' Detectamos si el nombre trae espacios
        iPos = InStr(sNombre, " ")

        If iPos > 0 Then
            If (Mid(sNombre, 1, iPos - 1) = "JOSE") Or (Mid(sNombre, 1, iPos - 1) = "MARIA") Then
                sNombre = Mid(sNombre, iPos + 1)
            End If
        End If

        fsDetectaNombre = sNombre
    End Function


    Private Function fsExtraePreposicion(ByVal sCadena As String) As String
        '******************************************************************
        '** Funcion:  fsExtraePreposicion                                **
        '** Objetivo: Esta funcion elimina las preposiciones dentro de   **
        '**           una cadena                                         **
        '**                                                              **
        '** Parametros de Entrada:                                       **
        '**         sCadena      - Cadena                                **
        '**                                                              **
        '** Parametros de Salida:                                        **
        '**         Regresa la cadena sin preposiciones                  **
        '******************************************************************
        Const PREPOSICIONES = 14

        Dim asPreposicion(PREPOSICIONES) As String
        Dim iPos As Integer
        Dim iIteracion As Integer
        Dim sCadenaSalida As String
        Dim sCadenaAux As String

        ' Asignamos las preposiciones que no se deben tomar en cuenta
        asPreposicion(0) = "DE"
        asPreposicion(1) = "DEL"
        asPreposicion(2) = "LA"
        asPreposicion(3) = "LOS"
        asPreposicion(4) = "LAS"
        asPreposicion(5) = "Y"
        asPreposicion(6) = "MC"
        asPreposicion(7) = "MAC"
        asPreposicion(8) = "VON"
        asPreposicion(9) = "VAN"
        asPreposicion(10) = "E"
        asPreposicion(11) = "AND"
        asPreposicion(12) = "&"
        asPreposicion(13) = "EN"

        ' Buscamos el primer espacio
        iPos = InStr(sCadena, " ")

        ' Iniciamos Cadena de salida
        sCadenaSalida = String.Empty

        If iPos > 0 Then
            ' La cadena trae espacios, buscamos las palabras validas
            While (iPos > 0)

                ' Extraemos la cadena antes del espacio
                sCadenaAux = Mid(sCadena, 1, iPos - 1)

                ' Verificamos si la palabra antes del espacio es una preposicion
                For iIteracion = 0 To PREPOSICIONES - 1
                    If sCadenaAux = asPreposicion(iIteracion) Then
                        sCadenaAux = ""
                        iIteracion = 10
                    End If
                Next

                sCadena = Mid(sCadena, iPos + 1)
                sCadenaSalida = sCadenaSalida + sCadenaAux

                If Len(sCadenaAux) > 0 Then
                    sCadenaSalida = sCadenaSalida + " "
                End If

                ' Buscamos el siguiente espacio
                iPos = InStr(sCadena, " ")
            End While
            sCadenaSalida = sCadenaSalida + sCadena
        Else
            ' No trae espacios, regresamos la cadena sin cambio alguno
            sCadenaSalida = sCadena
        End If

        ' Regresamos la cadena encontrada
        fsExtraePreposicion = sCadenaSalida
    End Function


    Public Function grabarTercero() As DataTable
        Dim oDatos As DataSet
        Dim oTabla As DataTable
        Dim oParametros As New Dictionary(Of String, Object)
        'Dim FecHoy As String
        Try
            'FecHoy = DateTime.Now.ToString("yyyyMMdd")
            'sFecUltModif = DateTime.Now.ToString("yyyyMMdd")
            'codTerceroAct = CInt(IIf(txt_codTercero.Text = "", 0, txt_codTercero.Text))
            'If txt_codTercero.Text <> Then oParametros.Add("clave", Clave)
            If iCodigoTercero <> 0 Then oParametros.Add("codTercero", iCodigoTercero)

            If sTipoPersona = "F" Then
                oParametros.Add("codTipoPersona", sTipoPersona)
                oParametros.Add("codOcupacion", -1)
                'If dSueldo <> -1 Then oParametros.Add("impSueldo", dSueldo)
                'oParametros.Add("txtSexo", sSexo)
                'oParametros.Add("fecNac", vFecNacim)
                'If sLugarNacim <> "" Then oParametros.Add("txtLugarNac", sLugarNacim)
                oParametros.Add("txtNombre", sNombre)
                oParametros.Add("txtApellido2", sApellido2)
            Else
                oParametros.Add("codTipoPersona", sTipoPersona)
                'oParametros.Add("txtSexo", "F")
            End If

            If dSueldo <> -1 Then oParametros.Add("impSueldo", dSueldo)
            oParametros.Add("txtSexo", sSexo)
            oParametros.Add("fecNac", vFecNacim)
            If sLugarNacim <> "" Then oParametros.Add("txtLugarNac", sLugarNacim)

            oParametros.Add("codUsuario", sCodUsuario)
            If sNombrePariente <> "" Then oParametros.Add("txtNomPariente", sNombrePariente)
            If sNroSSPariente <> "" Then oParametros.Add("txtRFCpariente", sNroSSPariente)
            oParametros.Add("nroEdad", iEdad)
            If sOcupacion <> "" Then oParametros.Add("txtOcupacion", sOcupacion)
            If sParentesco <> "" Then oParametros.Add("txtParentesco", sParentesco)
            oParametros.Add("txtApellido1", sApellido1)
            oParametros.Add("codTipoDoc", 1)
            oParametros.Add("nroDoc", sNit)
            oParametros.Add("nroNit", sNit)
            oParametros.Add("codEstCivil", iEstCivil)
            oParametros.Add("txtOrigen", sOrigen)


            Dim Direccion As String = sBarrio & "|" & sNroCasa & " " & icodCalle1 & " " & sNomCalle & " " & sBarrio


            oParametros.Add("codTipoDir", iTipoDir)
            oParametros.Add("codCalle1", icodCalle1)
            oParametros.Add("nroNro1", sNroCasa)
            oParametros.Add("nroApto", sOfic)
            oParametros.Add("txtDireccion", Direccion)
            oParametros.Add("nroCodPostal", scodPostal)
            oParametros.Add("codPpais", icodPais)
            oParametros.Add("codDdpto", icodDpto)
            oParametros.Add("codMunicipio", icodMun)
            oParametros.Add("txtNombreCalle", sNomCalle)
            oParametros.Add("txtNombreBarrio", sBarrio)
            If sTelCasa <> "" Then oParametros.Add("telcasa", sTelCasa)
            If sTelTrab <> "" Then oParametros.Add("teltrab", sTelTrab)
            If sCel <> "" Then oParametros.Add("celular", sCel)
            If sEmail <> "" Then oParametros.Add("email", sEmail)


            'Dim ssql As String
            'ssql = "execute usp_Alta_Upt_Tercero "
            'For Each p As KeyValuePair(Of String, Object) In oParametros
            '    'ssql = ssql + "@" + p.Key + " = " + p.Value.ToString() + ", "
            '    ssql = ssql + "@" + p.Key + " = " + IIf(IsNumeric(p.Value), p.Value.ToString(), "'" + p.Value.ToString() + "'") + ", "
            'Next

            oDatos = Funciones.ObtenerDatos("usp_Alta_Upt_Tercero", oParametros)
            'oDatos = Funciones.ObtenerDatos("usp_FJCP", oParametros)

            oTabla = oDatos.Tables(0)

            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                Return oTabla
            End If

            Return Nothing
        Catch ex As Exception
            MuestraMensaje("Exception", "Grabar: " & ex.Message, TipoMsg.Falla)
            Return Nothing
        End Try
    End Function


    Public Function ObtenerTercero(codTercero As Integer) As DataSet
        Dim oDatos As New DataSet
        Dim oParametros As New Dictionary(Of String, Object)
        Try
            oParametros.Add("cod_tercero", codTercero)
            oDatos = Funciones.ObtenerDatos("usp_obt_datos_tercero", oParametros)
            If Not oDatos Is Nothing AndAlso oDatos.Tables(0).Rows.Count > 0 Then
                Return oDatos
            End If

            Return Nothing
        Catch ex As Exception
            MuestraMensaje("Excepción", "Error: " + ex.Message, TipoMsg.Falla)
            Return Nothing
        End Try
    End Function

End Class
