$(document).ready(function () {
    PageLoadMaster();
});

//Evento Inicial en Master
function PageLoadMaster() {
    fn_EstadoVentanas();
    fn_EstadoGrid("gvd_Broker", "chk_SelBro");
    fn_EstadoGrid("gvd_Compañia", "chk_SelCia");
    fn_EstadoGrid("gvd_Poliza", "chk_SelPol");
    fn_EstadoGrid("gvd_RamoContable", "chk_SelRamC");
    fn_EstadoGrid("gvd_Producto", "chk_SelPro");
    fn_EstadoGrid("gvd_Ramo", "chk_SelRam");
    fn_EstadoGrid("gvd_Agrupador", "chk_SelAgr");

    $(".Fecha").mask("99/99/9999");

    $(".Fecha").datepicker({
        showOn: 'focus',
        buttonImageOnly: false,
        dateFormat: 'dd/mm/yy',
        numberOfMonths: 1,
        dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sa'],
        monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo',
            'Junio', 'Julio', 'Agosto', 'Septiembre',
            'Octubre', 'Noviembre', 'Diciembre'],
        monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr',
            'May', 'Jun', 'Jul', 'Ago',
            'Sep', 'Oct', 'Nov', 'Dic'],
        autoclose: true
    });


    //Botón Aceptar en Catalogo
    $("[id*=btn_Aceptar_Catalogo]").click(function () {

        var varSeleccion = '';
        var Filas = $("[id*=gvd_Catalogo]")[0].rows;

        for (i = 0; i <= Filas.length - 2; i++) {
            if ($('[id*=chk_Cat]')[i].checked == true) {
                varSeleccion = varSeleccion + Filas[i + 1].cells[1].innerText + '~' +
                    Filas[i + 1].cells[2].innerText + '~' +
                    Filas[i + 1].cells[3].innerText + '~' +
                    Filas[i + 1].cells[4].innerText + '~' +
                    Filas[i + 1].cells[5].innerText + '|';
            }
        }

        $("input[id$='hid_Seleccion']")[0].value = varSeleccion
        __doPostBack(this.name, '');
    });


    $("[id*=btn_Cancelar_Catalogo]").click(function () {
        event.preventDefault ? event.preventDefault() : event.returnValue = false;
        $("input[id$='hid_Control']")[0].value = '';
        $("input[id$='hid_Seleccion']")[0].value = '';
        $("input[id$='hid_Prefijo']")[0].value = '';
        fn_CerrarModal('#Catalogo');
    });

    //JIMENEZ
    $("[id*=btnTransferenciasBancariasCancelar_stro]").click(function () {
        event.preventDefault ? event.preventDefault() : event.returnValue = false;
        $("input[id$='hid_Control']")[0].value = '';
        fn_CerrarModal('#Transferencias_stro');
    });

    //JJIMENEZ
    $("[id*=btnCerrarTransferencias_stro]").click(function () {
        event.preventDefault ? event.preventDefault() : event.returnValue = false;
        $("input[id$='hid_Control']")[0].value = '';
        $("input[id$='hid_Seleccion']")[0].value = '';
        $("input[id$='hid_Prefijo']")[0].value = '';
        fn_CerrarModal('#Transferencias_stro');
    });

    //JIMENEZ
    $("[id*=btnCancelarRegistroTercero_stro]").click(function () {
        event.preventDefault ? event.preventDefault() : event.returnValue = false;
        $("input[id$='hid_Control']")[0].value = '';
        fn_CerrarModal('#CatalogoRegistroTerceros');
    });

    //JJIMENEZ
    $("[id*=btnCerrarRegistroTerceros_stro]").click(function () {
        event.preventDefault ? event.preventDefault() : event.returnValue = false;
        $("input[id$='hid_Control']")[0].value = '';
        $("input[id$='hid_Seleccion']")[0].value = '';
        $("input[id$='hid_Prefijo']")[0].value = '';
        fn_CerrarModal('#CatalogoRegistroTerceros');
    });

    $("#btn_Cerrar_Cat").click(function () {
        event.preventDefault ? event.preventDefault() : event.returnValue = false;
        $("input[id$='hid_Control']")[0].value = '';
        $("input[id$='hid_Seleccion']")[0].value = '';
        $("input[id$='hid_Prefijo']")[0].value = '';
        fn_CerrarModal('#Catalogo');
    });

    //Botón Mostrar Aclaración
    $("[id*=gvd_GrupoPolizas] .MuestraAclaracion").click(function () {
        event.preventDefault ? event.preventDefault() : event.returnValue = false;
        var row = $(this).closest("tr");
        var id_pv = row.find('.id_pv');
        fn_Aclaracion($(id_pv)[0].value);
    });


    //ToolTip para cualquier control
    //Establecer la propiedad title
    $('.masterTooltip').click(function () {
        // Hover over code
        var title = $(this).attr('title');
        $(this).data('tipText', title).removeAttr('title');
        $('<p class="tooltip"></p>')
            .text(title)
            .appendTo('body')
            .fadeIn('slow');
    }, function () {
        // Hover out code
        $(this).attr('title', $(this).data('tipText'));
        $('.tooltip').remove();
    }).mousemove(function (e) {
        var mousex = e.pageX + 20; //Get X coordinates
        var mousey = e.pageY + 10; //Get Y coordinates
        $('.tooltip')
            .css({ top: mousey, left: mousex })
    });

    //FORMATEO DE CAMPOS EN LA VISTA
    $(".Fecha").mask("99/99/9999");

    $(".cod").numeric({ decimal: false, negative: true, min: 0, max: 999999 });
    $(".cod").attr({ maxLength: 6 });
    $(".cod").css('text-align', 'center');

    $(".nro_pol").numeric({ decimal: false, negative: false, min: 0, max: 9999999 });
    $(".nro_pol").attr({ maxLength: 7 });
    $(".nro_pol").css('text-align', 'center');

    //$(".Monto").numeric({ precision: 18, scale: 4 });
    $(".Monto").numeric({ decimal: ".", negative: false, scale: 3 });
    //$(".Monto").numeric({ decimal: true, negative: false, min: 0, max: 9999999 });
    $(".Monto").css('text-align', 'right');

    $(".Prc").numeric({ decimal: ".", negative: false, scale: 2 });
    $(".Prc").css('text-align', 'right');
    $(".Prc").attr({ maxLength: 5 });

    $(".Centro").css('text-align', 'center');
    $(".Derecha").css('text-align', 'right');

    //Busqueda de Producto por Catalogo
    $("#btn_SelRam").click(function () {
        fn_CargaCatalogo("Pro", "", "", "Unica", "txtClaveRam|txtSearchRam", "Productos");
    });

    //Busqueda de Producto por Clave
    $("input[id$='txtClaveRam']").focusout(function () {
        var Id = $("input[id$='txtClaveRam']")[0].value;
        if (Id == "") {
            Id = 10000; //Coloca un número inexistente
        }
        $.ajax({
            url: "../LocalServices/ConsultaBD.asmx/GetProducto",
            data: "{ 'Id': " + Id + "}",
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                $("input[id$='txtSearchRam']")[0].value = data.d;
                $(".nro_pol").select();
            },
            error: function (response) {
                fn_MuestraMensaje('JSON', response.responseText, 2);
            },
        });
    });


    $('#txt_Filtro').keyup(function (event) {
        var searchKey = $(this).val().toLowerCase();
        var textoFila = ''

        if (searchKey.length > 1) {
            var Filas = $("[id*=gvd_Catalogo]")[0].rows;

            for (i = 0; i <= Filas.length - 2; i++) {
                var Clave = Filas[i + 1].cells[1].innerText.toLowerCase()
                var Descripcion = Filas[i + 1].cells[2].innerText.toLowerCase()
                var nro_nit = Filas[i + 1].cells[3].innerText.toLowerCase()

                if (Clave.indexOf(searchKey) >= 0 || Descripcion.indexOf(searchKey) >= 0) {
                    $(Filas[i + 1]).show();
                }
                else {
                    $(Filas[i + 1]).hide();
                }
            }
        }


        if ($('#txt_Filtro')[0].value == "") {
            $('[id$=gvd_Catalogo]').tablePagination({});
        }
    });

    //maximizar Ventana
    $("body").on("click", ".maximizar", function () {
        event.preventDefault ? event.preventDefault() : event.returnValue = false;
        var id = this.id.substr(this.id.length - 1)
        fn_Maximizar(id);
    });

    //restaurar Ventana
    $("body").on("click", ".restaurar", function () {
        event.preventDefault ? event.preventDefault() : event.returnValue = false;
        var id = this.id.substr(this.id.length - 1)
        fn_Restaurar(id);
    });


    //Busqueda de Producto por Catalogo
    $("#btn_SelCia").click(function () {
        fn_CargaCatalogo("Rea", "", "", "Unica", "txtClaveCia|txtSearchCia", "Compañias");
    });

    //Busqueda de Producto por Clave
    $("input[id$='txtClaveCia']").focusout(function () {
        var Id = $("input[id$='txtClaveCia']")[0].value;
        if (Id == "") {
            Id = 10000; //Coloca un número inexistente
        }
        $.ajax({
            url: "../LocalServices/ConsultaBD.asmx/GetCompañia",
            data: "{ 'Id': " + Id + "}",
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                $("input[id$='txtSearchCia']")[0].value = data.d;
            },
            error: function (response) {
                fn_MuestraMensaje('JSON', response.responseText, 2);
            },
        });
    });

    $("body").on("focus", "[id$=txtSearchCia]", function () {
        fn_Autocompletar("Rea", "txtClaveCia", "txtSearchCia", "", 0, -1)
        $(this).trigger({
            type: "keydown",
            which: 46
        });
    });

    $("body").on("focusout", "[id$=txtSearchCia]", function () {
        if ($(this)[0].value == '') {
            $("input[id$='txtClaveCia']")[0].value = ''
        }
        fn_EvaluaAutoComplete('txtClaveCia', 'txtSearchCia');

    });

    $("body").on("focus", ".Seleccion", function (e) {
        fn_Seleccion(this);
    });
}


//Ventana de Mensajes
function fn_MuestraMensaje(Titulo, Mensaje, Tipo, boton) {
    var lbl_TituloMensaje = document.getElementById('lbl_TituloMensaje');

    if (lbl_TituloMensaje != undefined) {
        document.getElementById('lbl_TituloMensaje').innerHTML = Titulo;
        document.getElementById('lbl_Mensaje').innerHTML = Mensaje;

        document.getElementById('img_advertencia').style.display = 'none';
        document.getElementById('img_confirma').style.display = 'none';
        document.getElementById('img_error').style.display = 'none';
        document.getElementById('img_pregunta').style.display = 'none';

        document.getElementById('btn_Si').style.display = 'none';
        document.getElementById('btn_No').style.display = 'none';

        //Almacena el control que desplego el Mensaje de Confirmación
        if (boton != undefined) {
            document.getElementById('hid_ControlASP').value = boton;
        }

        //Evalua el tipo de Mensaje
        switch (Tipo) {
            case 0:
                document.getElementById('img_advertencia').style.display = 'block';
                break;
            case 1:
                document.getElementById('img_confirma').style.display = 'block';
                break;
            case 2:
                document.getElementById('img_error').style.display = 'block';
                break;
            case 3:
                document.getElementById('img_pregunta').style.display = 'block';
                document.getElementById('btn_Si').style.display = 'inline-block';
                document.getElementById('btn_No').style.display = 'inline-block';
                break;
        }

        fn_AbrirModal('#Mensajes');
        //$('#Mensajes').draggable();
    }
    else {
        alert(Mensaje);
    }
}

//Respuesta del Usuario en Mensaje de Confirmación
function fn_Repuesta() {
    fn_CerrarModal('#Mensajes');
    __doPostBack(document.getElementById('hid_ControlASP').value, '');
}


//Respuesta del Usuario en Mensaje de Confirmación
function fn_Repuesta_Autoriza() {
    fn_CerrarModal('#Autorizacion');
    fn_AbrirModal('#EsperaModal');
    __doPostBack(document.getElementById('hid_controlAuto').value, '');
}

//Funciones de Consulta--------------------------------------------------------------------------------------------------------------------------------
function fn_CargaCatalogo(Catalogo, Condicion, Seleccion, Tipo, Control, Titulo, display_adicional) {

    $.ajax({
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        url: '../LocalServices/ConsultaBD.asmx/ObtieneDatos',
        data: "{ 'Consulta': 'spS_CatalogosSIR ==" + Catalogo + "==,==" + Condicion + "==" + Seleccion + "'}",
        dataType: 'JSON',
        success: function (response) {
            if (response.d.length > 0) {
                $("#lbl_Catalogo")[0].innerText = Titulo;
                $("input[id$='hid_Control']")[0].value = Control;
                $("input[id$='hid_Prefijo']")[0].value = Catalogo;
                fn_AbrirModal('#Catalogo');

                if (display_adicional == undefined) {
                    display_adicional = 'none';
                }

                $("[id*=gvd_Catalogo] tr").not($("[id*=gvd_Catalogo] tr:first")).remove();
                for (var i = 0; i < response.d.length; i++) {
                    $("[id*=gvd_Catalogo]").append('<tr>' +
                        '<td><input type="checkbox" id="chk_Cat" class="Select" onclick="fn_CambioSeleccion(' + "'gvd_Catalogo'" + ',this,' + "'" + Tipo + "','chk_Cat'" + ')" /></td>' +
                        '<td><label id="lbl_ClaveCat" class="texto-catalogo" style="Width:75px;">' + response.d[i].Clave + '</label></td>' +
                        '<td><label id="lbl_DesCat" class="texto-catalogo" style="Width:205px;">' + response.d[i].Descripcion + '</label></td>' +

                        '<td><label id="lbl_Oculta1" class="texto-catalogo" style="display:' + display_adicional + ';Width:105px">' + response.d[i].OcultaCampo1 + '</label></td>' +
                        '<td><label id="lbl_Oculta2" class="texto-catalogo" style="display:' + display_adicional + ';Width:105px">' + response.d[i].OcultaCampo2 + '</label></td>' +
                        '<td><label id="lbl_Oculta3" class="texto-catalogo" style="display:' + display_adicional + ';Width:105px">' + response.d[i].OcultaCampo3 + '</label></td>' +
                        '</tr>')
                };
                //Reference the GridView.
                var gridView = $("[id*=gvd_Catalogo]");

                //Reference the first row.
                var row = gridView.find("tr").eq(1);

                if ($.trim(row.find("td").eq(0).html()) == "") {
                    row.remove();
                }
                $('[id$=gvd_Catalogo]').tablePagination({});
                $('[id$=gvd_Catalogo]').each(function () {
                    $('tr:odd', this).addClass('odd').removeClass('even');
                    $('tr:even', this).addClass('even').removeClass('odd');
                })
                fn_CerrarModal('#EsperaModal');

            }
            else {
                //JJIMENEZ Si no existe el dato en el catalogo, se puede crear el dato en el catalogo
                switch (Catalogo) {
                    case "BenTercero_stro":
                        //VZAVALETA_10290_CC_INICIO
                        if  ($("[id*=hid_Alta_Tercero]").val() == 1) {
                            fn_MuestraMensaje('Catálogo', 'No se encontraron registros', 0);
                        } 
                        else {
                            CargarRegistroTerceros();
                            break;
                        }                                                   
                       //VZAVALETA_10290_CC_FIN
                    default:
                        fn_MuestraMensaje('Catálogo', 'No se encontraron registros', 0);
                        fn_CerrarModal('#EsperaModal');
                }
                //fn_MuestraMensaje('Catálogo', 'No se encontraron registros', 0);
                //fn_CerrarModal('#EsperaModal');
            }
        },
        error: function (e) {
            fn_MuestraMensaje('Catálogo', e.responseText, 2);
            fn_CerrarModal('#EsperaModal');
        }
    });
    return false;
};

//Función de Autocompletar
function fn_Autocompletar(Catalogo, ControlClave, ControlBusqueda, Condicion, minChar, caracter) {

    if (caracter != 13 && caracter != 9 && caracter != -1 && caracter != 37 && caracter != 38 && caracter != 39 && caracter != 40 && caracter != 46) {
        $("input[id$='" + ControlClave + "']")[0].value = '';
    }

    $("input[id$='" + ControlBusqueda + "']").css("color", "#555");

    var strSel = Condicion;

    $('[id$=' + ControlBusqueda + ']').autocomplete({
        minLength: minChar,
        source: function (request, response) {
            $.ajax({
                url: "../LocalServices/ConsultaBD.asmx/GetAutocompletar",
                data: "{ 'catalogo': '" + Catalogo + "' , 'prefix': '" + request.term + "' , 'strSel': '" + strSel + "'}",
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
                    fn_MuestraMensaje('JSON', response.responseText, 2);
                },
            });
        },
        select: function (e, i) {
            $("input[id$='" + ControlClave + "']")[0].value = i.item.val;
        }
    });
}

//Funcion para Seleccionar o Deseleccionar todos los elementos
function fn_SeleccionTodos(ControlGrid, chkControlAll, chkControl) {

    var controles = $("[id*=" + ControlGrid + "]").find("[id*=" + chkControl + "]");
    for (i = 0; i < controles.length; i++) {
        if (controles[i].isDisabled == false) {
            controles[i].checked = chkControlAll.checked;
        }
    }
}


//Cambio de selección de elemento en Catalogo
function fn_CambioSeleccion(ControlGrid, Control, TipoSeleccion, chkControl) {
    //Get target base & child control.

    var row = $(Control).closest("tr");

    var Grid = document.getElementById($('[id$=' + ControlGrid + ']')[0].id)

    //Evalua el tipo de seleccion
    if (TipoSeleccion == "Unica") {
        fn_SeleccionGread(Grid, false, chkControl)
        fn_SeleccionarElemento(ControlGrid, row[0].rowIndex)
    }
    return false;
}


function fn_SeleccionGread(Control, Valor, TargetChildControl) {
    if (Control == null) {
        fn_MuestraMensaje('no hay elementos', e.responseText, 2);
    }

    if (Control != null) {
        //Get all the control of the type INPUT in the base control.
        var Inputs = Control.getElementsByTagName("input");

        //Checked/Unchecked all the checkBoxes in side the GridView.
        for (var n = 0; n < Inputs.length; ++n)
            if (Inputs[n].type == 'checkbox' && Inputs[n].id.indexOf(TargetChildControl, 0) >= 0)
                if (Inputs[n].disabled == false) {
                    Inputs[n].checked = Valor;
                }
    }
    return false;
}


//Selecciona solo un elemento en caso de ser seleccion Unica
function fn_SeleccionarElemento(ControlGrid, rowIndex) {
    $("[id*=" + ControlGrid + "] tr").each(function (e) {
        var row = $(this).closest("tr");
        if (row[0].rowIndex == rowIndex) {
            var Select = row.find('.Select');
            if (Select.length > 0) {
                if ($(Select)[0].checked != undefined) {
                    $(Select)[0].checked = true;
                }
                else {
                    $(Select)[0].childNodes[0].checked = true;
                }
            }
        }
    })
    return false;
}

//Abrir Modal
function fn_AbrirModal(modal) {
    $(modal).modal('show');
}

//Cerrar Modal
function fn_CerrarModal(modal) {
    $(modal).modal('hide');
}

function fn_AbrirModalSimple(modal) {
    fn_Desplazable(modal);
    $(modal).css("display", "block");
}

function fn_CerrarModalSimple(modal) {
    $(modal).css("display", "none");
}

//Cambia Estado de Ventana
function fn_CambiaEstado(IdControl, Colapsado) {
    $('.ventana' + IdControl).slideToggle();

    var Ventana = $("input[id$='hid_Ventanas']")[0].value;
    var Estado = Ventana.split("|");

    Estado[IdControl] = Colapsado

    $("input[id$='hid_Ventanas']")[0].value = "";

    for (i = 0; i < Estado.length - 1; i++) {
        $("input[id$='hid_Ventanas']")[0].value = $("input[id$='hid_Ventanas']")[0].value + Estado[i] + "|";
    }

    if (Colapsado == "1") {
        $("#coVentana" + IdControl).hide();
        $("#exVentana" + IdControl).show();
    }
    else {
        $("#coVentana" + IdControl).show();
        $("#exVentana" + IdControl).hide();
    }
}

//Funciones Estado-------------------------------------------------------------------------------------------------------------------------------------
function fn_EstadoVentanas() {
    var Ventana = $("input[id$='hid_Ventanas']")[0].value;
    var Estado = Ventana.split("|");

    for (i = 0; i < Estado.length - 1; i++) {
        if (Estado[i] == "1") {
            $('.ventana' + i).hide();
            $("#coVentana" + i).hide();
            $("#exVentana" + i).show();
        }
        else {
            $('.ventana' + i).show();
            $("#coVentana" + i).show();
            $("#exVentana" + i).hide();
        }
    }
}

function fn_EstadoGrid(Grid, Control) {
    if ($("[id*=" + Grid + "]")[0] != undefined) {
        var Rows = $("[id*=" + Grid + "]")[0].rows;
        for (i = 0; i <= Rows.length - 2; i++) {
            if ($('[id*=' + Control + ']')[i] != undefined) {
                if ($('[id*=' + Control + ']')[i].value == "true") {
                    var row = $('[id*=' + Control + ']')[i].parentNode.parentNode;
                    row.style.display = "none";
                }
            }
        }
    }
}

//Evalua los elementos ya seleccionados para una Consulta
function fn_ElementosSeleccionados(Gread, Control, Seleccion, blnTexto) {
    var caracter = '';

    if (blnTexto == true) { caracter = '===='; }

    var strSel = caracter + '-1' + caracter;

    if (Gread.length > 0) {
        var Filas = Gread[0].rows;
        for (i = 0; i <= Filas.length - 2; i++) {
            var Clave = Control[i].innerText;
            var chk_Sel = Seleccion[i].value

            //Verifica que no haya sido descartado de la lista
            if (chk_Sel != 'true') { strSel = strSel + ',' + caracter + Clave + caracter; }
        }
    }

    if (strSel == caracter + '-1' + caracter) { strSel = ''; }
    else { strSel = ",==" + strSel + "=="; }

    return strSel;
}


//Funciones de Seleccion-------------------------------------------------------------------------------------------------------------------------------
function fn_Seleccion(Control) {
    $(Control).focus(function () {
        this.select();
    });
}

function fn_Aclaracion(id_pv) {
    $.ajax({
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        url: '../LocalServices/ConsultaBD.asmx/GetAclaracion',
        data: "{ 'id_pv': '" + id_pv + "'}",
        dataType: 'JSON',
        success: function (response) {
            if (response.d.length > 0) {
                fn_CerrarModal('#EsperaModal');
                $(".Aclaracion")[0].innerHTML = response.d;
                fn_AbrirModal('#AclaracionesModal');
            }
            else {
                fn_CerrarModal('#EsperaModal');
                fn_MuestraMensaje('Aclaraciones', 'No se encontraron registros', 0);
            }
        },
        error: function (e) {
            fn_CerrarModal('#EsperaModal');
            fn_MuestraMensaje('JSON', e.responseText, 2);
        }
    });
    return false;
};

function fn_Desplazable(modal) {
    $(modal).draggable({ disabled: false });
}

function fn_Resizable(modal) {
    $(modal).resizable();
}

function fn_NoDesplazable(control, control_base) {
    var left = $(control_base).css("left");
    var top = $(control_base).css("top");

    $(control).css({
        'left': left,
        'top': top
    });
    $(control).draggable({ disabled: true });
}

//Formato de comas a N posiciones decimales
function fn_FormatoMonto(Monto, decimales, porcentaje) {
    if (isNaN(Monto) == true) {
        return '0.00';
    }
    else {
        if (porcentaje == undefined) {
            return Monto.toFixed(decimales).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
        }
        else {
            return Monto.toFixed(decimales)
        }
    }
}

//Evalua tecla Numerica
function fn_EvaluaNumerico(keynum) {
    if ((keynum == 8) || (keynum == 46))
        return true;

    return /\d/.test(String.fromCharCode(keynum));
}

function fn_EvaluaAutoComplete(ControlClave, ControlDescripcion) {
    if ($("input[id$='" + ControlClave + "']")[0].value == "") {
        $("input[id$='" + ControlDescripcion + "']").css("color", "red");
    }
    else {
        $("input[id$='" + ControlDescripcion + "']").css("color", "#555");
    }
}


function fn_DateDiff(date1, date2, interval) {
    var date = date1.split('/');
    date1 = date[2] + '-' + date[1] + '-' + date[0]

    var date = date2.split('/');
    date2 = date[2] + '-' + date[1] + '-' + date[0]

    var second = 1000, minute = second * 60, hour = minute * 60, day = hour * 24, week = day * 7;
    date1 = new Date(date1);
    date2 = new Date(date2);
    var timediff = date2 - date1;
    if (isNaN(timediff)) return NaN;
    switch (interval) {
        case "year": return date2.getFullYear() - date1.getFullYear();
        case "month": return (
            (date2.getFullYear() * 12 + date2.getMonth())
            -
            (date1.getFullYear() * 12 + date1.getMonth())
        );
        case "week": return Math.floor(timediff / week);
        case "day": return Math.floor(timediff / day);
        case "hour": return Math.floor(timediff / hour);
        case "minute": return Math.floor(timediff / minute);
        case "second": return Math.floor(timediff / second);
        default: return undefined;
    }
}

function fn_IsDate(date) {
    return (new Date(date) !== "Invalid Date" && !isNaN(new Date(date))) ? true : false;
}


function fn_MovimientoFlechas(tecla, ctrIzq, ctrDer, ctrAba, ctrArr) {
    if ((tecla == 37 && ctrIzq != undefined) || (tecla == 38 && ctrArr != undefined) || (tecla == 39 && ctrDer != undefined) || (tecla == 40 && ctrAba != undefined)) {
        switch (tecla) {
            case 37:
                $("[id$='" + ctrIzq + "']").focus();
                break;
            case 38:
                $("[id$='" + ctrArr + "']").focus();
                break;
            case 39:
                $("[id$='" + ctrDer + "']").focus();
                break;
            case 40:
                $("[id$='" + ctrAba + "']").focus();
                break;
        }
        return 1;
    }
    else {
        return 0;
    }
}

//function fn_MovimientoFlechasGrid(tecla, ctrIzq, ctrDer) {
//    if ((tecla == 37 && ctrIzq != undefined) ||  (tecla == 39 && ctrDer != undefined)) {
//        switch (tecla) {
//            case 37:
//                ctrIzq.focus();
//                break;
//            case 39:
//                ctrDer.focus();
//                break;
//        }
//        return 1;
//    }
//    else {
//        return 0;
//    }
//}

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
}

function fn_MuestraMensajeCorreo() {
    $("[id*=gvd_Correos] .Mensaje").each(function (e) {
        var row = $(this).closest("tr");
        $(this)[0].innerHTML = row.find(".MensajeAux")[0].value
        row.find(".MensajeAux")[0].value = "";
    });
}


function fn_Maximizar(id) {
    $('.Marco' + id).css("width", "97.5%");
    $('.Marco' + id).addClass("modal-completa");
    $('.Marco' + id).modal('show');

    $("#maVentana" + id).hide();
    $("#reVentana" + id).show();

    if ($("#exVentana" + id).is(':visible') == true) {
        fn_CambiaEstado(id, "0");
    }
    $("#coVentana" + id).hide();
}

function fn_Restaurar(id) {
    $('.Marco' + id).removeClass("modal-completa");
    $('.Marco' + id).modal('hide');
    $('.Marco' + id).css("width", "100%");
    $('.Marco' + id).show();

    $("#maVentana" + id).show();
    $("#reVentana" + id).hide();
    $("#coVentana" + id).show();
}

//Imprimir Ordenes de Pago
function fn_Imprime_OP(Server, strOrden) {
    var nro_op = strOrden.split(",");
    for (i = 0; i < nro_op.length; i++) {
        window.open(Server.replace('@nro_op', nro_op[i]));
    }
}

//Imprimir Soporte de Ordenes de Pago
function fn_Imprime_SoporteOP(Server, strOrden) {
    var nro_op = strOrden.split(",");
    for (i = 0; i < nro_op.length; i++) {
        window.open(Server.replace('@nro_op', nro_op[i]));
    }
}

//Imprimir Soporte de Ordenes de Pago
function fn_Imprime_Reporte(url) {
    window.open(url.replace(/\+/g, "'"));
}

//JJIMENEZ
function CargarRegistroTerceros() {
    fn_AbrirModal('#EsperaModal');
    fn_AbrirModal('#RegistroTerceros'); //FCJP MEJORAS 10290 REGISTRO DE TERCEROS DATOS MINIMOS
    fn_CerrarModal('#EsperaModal');
};

function obtenerEdad() {
    let hoy = new Date();
    var fechaNac = $("[id*=txt_fecNacmTer]").val();
    var fechaNacFto = fechaNac.substring(3, 5) + "/" + fechaNac.substring(0, 2) + "/" + fechaNac.substring(6, 10);
    let fechaNacimiento = new Date(fechaNacFto);
    let edad = hoy.getFullYear() - fechaNacimiento.getFullYear();
    let diferenciaMeses = hoy.getMonth() - fechaNacimiento.getMonth();
    if (diferenciaMeses < 0 || (diferenciaMeses === 0 && hoy.getDate() < fechaNacimiento.getDate())) {
        edad--;
    }
    $("[id*=hidEdadmTer]").val(edad)
}


function convMayusculas(control) {
    var str = $("[id*=" + control + "]").val();
    var strMayus = str.toUpperCase();
    $("[id*=" + control + "]").val(strMayus);
}


function selecTercero() {

    //alert("Alerta JavaScript")
    var codTercero = $("[id*=hidCodTercero]").val();
    var nomTercero = $("[id*=hidNomTercero]").val();
    var rfcTercero = $("[id*=hidrfcTercero]").val();

    $("[id*=txtCodigoBeneficiario_stro]").val(codTercero);
    $("[id*=txtBeneficiario_stro]").val(nomTercero);
    $("[id*=txtRFC]").val(rfcTercero);
    
}



function llenarCpto2() {
    var cpto2 = $("[id*=HiddenFieldPI]").val();
    $("[id*=txtcpto2]").val(cpto2);
}


function cuenta(e, control) {
    var key;
    if (window.event) // IE
    {
        key = e.keyCode;
    }
    else if (e.which) // Netscape/Firefox/Opera
    {
        key = e.which;
    }

    //if (key < 48 || key > 57) {
    //    return false;
    //}

    var tipoCuenta = $("[id*=cmbTipoCuentaT_stro]").val();

    if (tipoCuenta == 2) {
        if (LengthCheck(control)) {
            return true;
        }
        else {
            return false;
        }
    }
}


function LengthCheck(control) {
    var dato = $("[id*=" + control + "]").val();
    var long = dato.length;

    if (long > 17) {
        return false;
    }
    return true;
}

function validaLong(control) {
    var dato = $("[id*=" + control + "]").val();
    var long = dato.length;
    var tipoCuenta = $("[id*=cmbTipoCuentaT_stro]").val();

    if (tipoCuenta == 2) {
        if (long < 17) {
            fn_MuestraMensaje('Validación', 'Se deben capturar 18 digitos', 2);
            $("[id*=" + control + "]").val("");
        }
    }
}