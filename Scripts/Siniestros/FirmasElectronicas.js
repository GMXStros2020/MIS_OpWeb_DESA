$(document).ready(function () {
    $("#lbl_Titulo").text("SINIESTROS");
});

function PageLoadFirmas() {
    fn_EstadoFilas('grdOrdenPago', false);
}

function fn_EstadoFilas(ControlGrid, blnVentana) {
    $("[id*=" + ControlGrid + "] .Estado").each(function (e) {
        
        var row = $(this).closest("tr");

        var Ventana = row.find('.Ventana');
        var Ocultar = row.find('.Ocultar');
        var Mostrar = row.find('.Mostrar');
        var Estado = row.find('.Estado');
        
        if (Estado.length > 0) {
            if (Estado[0].value == 1) {
                if (blnVentana == true) {
                    Ventana.slideToggle();
                }
                Mostrar.show();
                Ocultar.hide();
            }
            else {
                Mostrar.hide();
                Ocultar.show();
            }
        }
    })
};

function fn_Imprime_OP(Server, strOrden) {
    var nro_op = strOrden.split(",");
    for (i = 0 ; i < nro_op.length; i++) {
        window.open(Server.replace('@nro_op', nro_op[i]));
    }
};

//Imprimir Soporte de Ordenes de Pago
function fn_Imprime_SoporteOP(Server, strOrden) {
    var nro_op = strOrden.split(",");
    for (i = 0 ; i < nro_op.length; i++) {
        window.open(Server.replace('@nro_op', nro_op[i]));
    }
}

//Detecta evento de Confirmación en Controles con dicha Clase
function fn_Autoriza() {
    $("input[id$='hid_controlAuto']")[0].value = 'ctl00$cph_principal$btn_Confirmar';
    fn_AbrirModal('#Autorizacion');
};

$("body").on("click", "[id*=gvd_ListaCorreo] .Correo", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var row = $(this).closest("tr");
    var btnCorreo = row.find('.btnCorreo');
    __doPostBack(btnCorreo[0].name, '');
    return false;
});

////////////////////////////////////////////////////////////////////EVENTO EXPANDIR-CONTRAER/////////////////////////////////////
//Colapsar Ventana
$("body").on("click", ".contraer", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var id = this.id.substr(this.id.length - 1)
    fn_CambiaEstado(id, "1");
});

//Expandir Ventana
$("body").on("click", ".expandir", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var id = this.id.substr(this.id.length - 1)
    fn_CambiaEstado(id, "0");
});
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

$("body").on("click", "[id*=grdOrdenPago] .Mostrar", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var row = $(this).closest("tr");
    var Ventana = row.find('.Ventana');
    var Estado = row.find('.Estado');

    Estado[0].value = 0;
    Ventana.slideToggle();

    $(this).hide();

    var Ocultar = row.find('.Ocultar');
    Ocultar.show();
});

$("body").on("click", "[id*=grdOrdenPago] .Ocultar", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var row = $(this).closest("tr");
    var Ventana = row.find('.Ventana');
    var Estado = row.find('.Estado');

    Estado[0].value = 1;
    Ventana.slideToggle();

    $(this).hide();

    var Ocultar = row.find('.Mostrar');
    Ocultar.show();
});


$("body").on("click", ".AgregaUsuario", function () {
    var strSel = fn_ElementosSeleccionados($("[id*=gvd_Usuario]"), $('[id*=lbl_ClaveUsu]'), $('[id*=chk_SelUsu]'), true);
    fn_CargaCatalogo("UsuStro", "", strSel, "Multiple", "gvd_Usuario", "USUARIOS");
});

$("body").on("click", ".AgregaEstatus", function () {
    var strSel = fn_ElementosSeleccionados($("[id*=gvd_Estatus]"), $('[id*=lbl_ClaveEst]'), $('[id*=chk_SelEst]'), true);
    fn_CargaCatalogo("EstatusOP_stro", "", strSel, "Multiple", "gvd_Estatus", "ESTATUS");
});

//Detecta la clase Agregar Broker y abre el Catalogo
$("body").on("click", ".AgregaBroker", function () {
    var strSel = fn_ElementosSeleccionados($("[id*=gvd_Broker]"), $('[id*=lbl_ClaveBro]'), $('[id*=chk_SelBro]'), false);

    //*************fn_CargaCatalogo(PrefijoCatalogo,Condicion,Seleccion,TipoSeleccion,IdGrid,Titulo)***************
    fn_CargaCatalogo("Bro", "", strSel, "Multiple", "gvd_Broker", "BROKERS");
});

//Detecta la clase Agregar Compañia y abre el Catalogo
$("body").on("click", ".AgregaCia", function () {
    var strSel = fn_ElementosSeleccionados($("[id*=gvd_Compañia]"), $('[id*=lbl_ClaveCia]'), $('[id*=chk_SelCia]'), false);

    //*************fn_CargaCatalogo(PrefijoCatalogo,Condicion,Seleccion,TipoSeleccion,IdGrid,Titulo)***************
    fn_CargaCatalogo("Cia", "", strSel, "Multiple", "gvd_Compañia", "COMPAÑIAS");
});

$("body").on("click", ".AgregaRamoCont", function () {
    var strSel = fn_ElementosSeleccionados($("[id*=gvd_RamoContable]"), $('[id*=lbl_ClaveRamC]'), $('[id*=chk_SelRamC]'), true);
    fn_CargaCatalogo("RamC", "", strSel, "Multiple", "gvd_RamoContable", "RAMOS CONTABLES");
});

$("body").on("click", ".AgregaContrato", function () {
    var strSel = fn_ElementosSeleccionados($("[id*=gvdContrato]"), $('[id*=lbl_ClaveCto]'), $('[id*=chk_SelCto]'), false);
    var Fecini = $("input[id$='txtFecIni']")[0].value;
    var Fecfin = $("input[id$='txtFecFin']")[0].value;
    var Ejercicio = $("input[id$='hidEjercicio']")[0].value;
    if (Fecini != "") {
        var Condicion = 'Where fec_vig_desde >=|==' + Fecini + '==| and fec_vig_hasta <=|==' + Fecfin + '==| and aaaa_ejercicio=' + Ejercicio;
        fn_CargaCatalogo("CtrN", Condicion, strSel, "Multiple", "gvdContrato", "Contratos");
    }
    else {
        fn_MuestraMensaje('Validación', 'Se debe indicar Rango de fechas y Ejercicio', 0, "")
        return false;
    }
});

$("body").on("click", ".AgregaCtoL", function () {
    var strSel = fn_ElementosSeleccionados($("[id*=gvdCtoLocal]"), $('[id*=lbl_ClaveCtoL]'), $('[id*=chk_SelCtoL]'), false);
    var Fecini = $("input[id$='txtFecIni']")[0].value;
    var Fecfin = $("input[id$='txtFecFin']")[0].value;
    var Ejercicio = $("input[id$='hidEjercicio']")[0].value;
    if (Fecini != "") {
        var Condicion = 'And fec_vig_desde >=|==' + Fecini + '==| and fec_vig_hasta <=|==' + Fecfin + '==| and aaaa_ejercicio=' + Ejercicio;
        fn_CargaCatalogo("CtrL", Condicion, strSel, "Multiple", "gvdCtoLocal", "Contratos Local");
    }
    else {
        fn_MuestraMensaje('Validación', 'Se debe indicar Rango de fechas y Ejercicio', 0, "")
        return false;
    }
});


//Delete event handler.
$("body").on("click", "[id*=gvd_Usuario] .Delete", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var row = $(this).closest("tr");
    $('[id*=chk_SelUsu]')[row[0].rowIndex - 1].value = "true";
    row.hide();
    return false;
});

//Delete event handler.
$("body").on("click", "[id*=gvd_Estatus] .Delete", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var row = $(this).closest("tr");
    $('[id*=chk_SelEst]')[row[0].rowIndex - 1].value = "true";
    row.hide();
    return false;
});

//Delete event handler.
$("body").on("click", "[id*=gvd_Broker] .Delete", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var row = $(this).closest("tr");
    $('[id*=chk_SelBro]')[row[0].rowIndex - 1].value = "true";
    row.hide();
    return false;
});

//Delete event handler.
$("body").on("click", "[id*=gvd_Compañia] .Delete", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var row = $(this).closest("tr");
    $('[id*=chk_SelCia]')[row[0].rowIndex - 1].value = "true";
    row.hide();
    return false;
});

//Delete event handler.
$("body").on("click", "[id*=gvd_Poliza] .Delete", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var row = $(this).closest("tr");
    $('[id*=chk_SelPol]')[row[0].rowIndex - 1].value = "true";
    row.hide();
    return false;
});

//Delete event handler.
$("body").on("click", "[id*=gvd_RamoContable] .Delete", function () {
    event.preventDefault ? event.preventDefault() : event.returnValue = false;
    var row = $(this).closest("tr");
    $('[id*=chk_SelRamC]')[row[0].rowIndex - 1].value = "true";
    row.hide();
    return false;
});

$("body").on("keydown", "[id$=txtSearchAse]", function (e) {

    if (e.which != 13) {
        $("input[id$='hidClaveAse']")[0].value = "";
    }

    $('[id$=txtSearchAse]').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "../LocalServices/ConsultaBD.asmx/GetAsegurado",
                data: "{ 'prefix': '" + request.term + "'}",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data.d, function (item) {
                        return {
                            label: item.split('|')[0],
                            val: item.split('|')[1]
                        }
                    }))
                },
                error: function (response) {
                    EvaluaMensaje('JSON', response.responseText);
                },
            });
        },
        select: function (e, i) {
            $("input[id$='hidClaveAse']")[0].value = i.item.val;
        },
        minLength: 1
    });
});

$("body").on("keydown", "[id$=txtSearchEve]", function (e) {

    if (e.which != 13) {
        $("input[id$='hidClaveEve']")[0].value = "";
    }

    $('[id$=txtSearchEve]').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "../LocalServices/ConsultaBD.asmx/GetEventoCat",
                data: "{ 'prefix': '" + request.term + "'}",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data.d, function (item) {
                        return {
                            label: item.split('|')[0],
                            val: item.split('|')[1]
                        }
                    }))
                },
                error: function (response) {
                    EvaluaMensaje('JSON', response.responseText);
                },
            });
        },
        select: function (e, i) {
            $("input[id$='hidClaveEve']")[0].value = i.item.val;
        },
        minLength: 1
    });
});


//Eventos Focus----------------------------------------------------------------------------------------------------------------------------------------
$("body").on("focus", "[id*=gvd_Contabilidad] .prcPrima", function () {
    fn_Seleccion($(this)[0]);
});

$("body").on("focus", "[id*=gvd_Contabilidad] .Prima", function () {
    fn_Seleccion($(this)[0]);
});

$("body").on("focus", "[id*=gvd_Contabilidad] .prcComision", function () {
    fn_Seleccion($(this)[0]);
});

$("body").on("focus", "[id*=gvd_Contabilidad] .Comision", function () {
    fn_Seleccion($(this)[0]);
});
//-----------------------------------------------------------------------------------------------------------------------------------------------------

//Eventos FocusOut-------------------------------------------------------------------------------------------------------------------------------------
$("body").on("focusout", "[id*=gvd_Contabilidad] .prcPrima", function () {
    var prcPrima = parseFloat($(this)[0].value.replace(",", "").replace(",", ""))
    $(this)[0].value = fn_FormatoMonto(prcPrima, 4);

    //Calculo de Prima en Base al Procentaje
    fn_CalculaPrima($(this).closest("tr"));

    //Caluculo de Prima Neta
    fn_CalculaPrimaNeta($(this).closest("tr"));

    //Calculo de ISR
    fn_CalculaISR($(this).closest("tr"))

    //Suma de Totales
    fn_SumaTotalesOP()
});

$("body").on("focusout", "[id*=gvd_Contabilidad] .Prima", function () {
    var Prima = parseFloat($(this)[0].value.replace(",", "").replace(",", ""))
    $(this)[0].value = fn_FormatoMonto(Prima, 4);

    //Calculo de Prc en Base a Prima
    fn_CalculaPrcPrima($(this).closest("tr"));

    //Caluculo de Prima Neta
    fn_CalculaPrimaNeta($(this).closest("tr"));

    //Calculo de ISR
    fn_CalculaISR($(this).closest("tr"))

    //Suma de Totales
    fn_SumaTotalesOP()
});

$("body").on("focusout", "[id*=gvd_Contabilidad] .prcComision", function () {
    var prcComision = parseFloat($(this)[0].value.replace(",", "").replace(",", ""))
    $(this)[0].value = fn_FormatoMonto(prcComision, 4);

    //Calculo de Comision en Base al Procentaje
    fn_CalculaComision($(this).closest("tr"));

    //Caluculo de Prima Neta
    fn_CalculaPrimaNeta($(this).closest("tr"));

    //Suma de Totales
    fn_SumaTotalesOP()
});

$("body").on("focusout", "[id*=gvd_Contabilidad] .Comision", function () {
    var Comision = parseFloat($(this)[0].value.replace(",", "").replace(",", ""))
    $(this)[0].value = fn_FormatoMonto(Comision, 4);

    //Calculo de Prc en Base a Comision
    fn_CalculaPrcComision($(this).closest("tr"));

    //Caluculo de Prima Neta
    fn_CalculaPrimaNeta($(this).closest("tr"));

    //Suma de Totales
    fn_SumaTotalesOP()
});
//-----------------------------------------------------------------------------------------------------------------------------------------------------


//Eventos KeyPress-------------------------------------------------------------------------------------------------------------------------------------
$("body").on("keypress", "[id*=gvd_Contabilidad] .prcPrima", function (e) {
    var keynum = window.event ? window.event.keyCode : e.which;
    return fn_EvaluaNumerico(keynum);
});
$("body").on("keypress", "[id*=gvd_Contabilidad] .Prima", function (e) {
    var keynum = window.event ? window.event.keyCode : e.which;
    return fn_EvaluaNumerico(keynum);
});
$("body").on("keypress", "[id*=gvd_Contabilidad] .prcComision", function (e) {
    var keynum = window.event ? window.event.keyCode : e.which;
    return fn_EvaluaNumerico(keynum);
});
$("body").on("keypress", "[id*=gvd_Contabilidad] .Comision", function (e) {
    var keynum = window.event ? window.event.keyCode : e.which;
    return fn_EvaluaNumerico(keynum);
});
//-----------------------------------------------------------------------------------------------------------------------------------------------------


//Funciones de Calculo---------------------------------------------------------------------------------------------------------------------------------
//Calculo de Prima en Base a Porcentaje
function fn_CalculaPrima(Fila) {
    //Obtiene Valores
    var prcPrima = Fila.find('.prcPrima').val();
    var PrimaCedida = Fila.find('.PrimaCedida').val();

    //Obtiene Controles
    var txt_Prima = Fila.find('.Prima');
    var txt_prcCom = Fila.find('.prcComision');

    //Elimina comas en caso de existir
    PrimaCedida = PrimaCedida.replace(",", "").replace(",", "");
    prcPrima = prcPrima.replace(",", "").replace(",", "");

    //Calcula en base al Porcentaje
    var Calculo = PrimaCedida * (prcPrima / 100);

    //Coloca el Resultado
    $(txt_Prima)[0].value = fn_FormatoMonto(Calculo, 4);


    //Replica el valor en Comisión
    $(txt_prcCom)[0].value = prcPrima;
    fn_CalculaComision(Fila);
}

//Calculo de Prc en Base a Prima
function fn_CalculaPrcPrima(Fila) {
    //Obtiene Valoes
    var PrimaCedida = Fila.find('.PrimaCedida').val();
    var Prima = Fila.find('.Prima').val();

    //Obtiene Controles
    var txt_prcPrima = Fila.find('.prcPrima');
    var txt_prcCom = Fila.find('.prcComision');

    //Elimina comas en caso de existir
    PrimaCedida = PrimaCedida.replace(",", "").replace(",", "");
    Prima = Prima.replace(",", "").replace(",", "");

    //Calcula en base a Prima
    var Calculo = (Prima * 100) / PrimaCedida;

    $(txt_prcPrima)[0].value = fn_FormatoMonto(Calculo, 4);

    //Replica el valor en Comisión
    $(txt_prcCom)[0].value = $(txt_prcPrima)[0].value;
    fn_CalculaComision(Fila);
}


//Calculo de Comision en Base a Porcentaje
function fn_CalculaComision(Fila) {
    //Obtiene Valores
    var prcComision = Fila.find('.prcComision').val();
    var Comisiones = Fila.find('.Comisiones').val();

    //Obtiene Controles
    var txt_Comision = Fila.find('.Comision');

    //Elimina comas en caso de existir
    Comisiones = Comisiones.replace(",", "").replace(",", "");
    prcComision = prcComision.replace(",", "").replace(",", "");

    //Calcula en base al Porcentaje
    var Calculo = Comisiones * (prcComision / 100);

    //Coloca el Resultado
    $(txt_Comision)[0].value = fn_FormatoMonto(Calculo, 4);
}


//Calculo de Prc en Base a Comision
function fn_CalculaPrcComision(Fila) {
    //Obtiene Valoes
    var Comisiones = Fila.find('.Comisiones').val();
    var Comision = Fila.find('.Comision').val();

    //Obtiene Controles
    var txt_prcCom = Fila.find('.prcComision');

    //Elimina comas en caso de existir
    Comisiones = Comisiones.replace(",", "").replace(",", "");
    Comision = Comision.replace(",", "").replace(",", "");

    //Calcula en base a Prima
    var Calculo = (Comision * 100) / Comisiones;

    $(txt_prcCom)[0].value = fn_FormatoMonto(Calculo, 4);
}


//Calculo de Prima Neta
function fn_CalculaPrimaNeta(Fila) {
    //Obtiene Valores
    var Prima = Fila.find('.Prima').val();
    var Comision = Fila.find('.Comision').val();

    //Obtiene Controles
    var lbl_PrimaNeta = Fila.find('.PrimaNeta');

    //Elimina comas en caso de existir
    Prima = Prima.replace(",", "").replace(",", "");
    Comision = Comision.replace(",", "").replace(",", "");

    var Calculo = Prima - Comision;

    $(lbl_PrimaNeta)[0].value = fn_FormatoMonto(Calculo, 4);
}

//Calculo de ISR
function fn_CalculaISR(Fila) {

    //Obtiene Valores
    var Prima = Fila.find('.Prima').val();
    var prcISR = Fila.find('.prcISR').val();

    //Obtiene Controles
    var lbl_MontoISR = Fila.find('.ISR');

    //Elimina comas en caso de existir
    Prima = Prima.replace(",", "").replace(",", "");
    prcISR = prcISR.replace(",", "").replace(",", "");

    var Calculo = Prima * (prcISR / 100);

    $(lbl_MontoISR)[0].value = fn_FormatoMonto(Calculo, 4);
}

//Suma de Montos Totales
function fn_SumaTotalesOP() {
    var TotalPrimaNeta = parseFloat(0);
    var TotalISR = parseFloat(0);

    $("[id*=gvd_Contabilidad] tr").each(function (e) {
        var Fila = $(this).closest("tr");

        var PrimaNeta = Fila.find('.PrimaNeta').val();
        var ISR = Fila.find('.ISR').val();

        if (PrimaNeta != undefined && ISR != undefined) {
            PrimaNeta = PrimaNeta.replace(",", "").replace(",", "");
            ISR = ISR.replace(",", "").replace(",", "");

            TotalPrimaNeta = TotalPrimaNeta + parseFloat(PrimaNeta);
            TotalISR = TotalISR + parseFloat(ISR);
        }
    });

    $("input[id$='lbl_MontoPago']")[0].value = fn_FormatoMonto(TotalPrimaNeta - TotalISR, 4);
    $("input[id$='lbl_Impuesto']")[0].value = fn_FormatoMonto(TotalISR, 4);
}

function fn_Abrir_Archivo(documento) {
    window.open('file:' + documento);
}
//>VZAVALETA_10290_CC7_PDF
function fn_abrir_documento(documento) {
    window.open('file:' + documento);
}
//-----------------------------------------------------------------------------------------------------------------------------------------------------