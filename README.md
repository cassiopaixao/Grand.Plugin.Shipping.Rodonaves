# Grandnode Plugin Shipping RTE Rodonaves

Plugin to compute shipping rate for [RTE Rodonaves](https://www.rte.com.br) service. Tested for GrandNode v4.60.

Keep in mind that this plugin is only intented to use the [/api/v1/simula-cotacao](https://dev.rte.com.br/#!/Cotação/Quotation_SimulateQuotation) endpoint. There is no support to generate a quote or track a package.

Note: The main structure was inspired by the [Correios shipping plugin](https://github.com/rafaelboschini/Grand.Plugin.Shipping.Correios) (_Thanks, [rafaelboschini](https://github.com/rafaelboschini) !_)

# Install

You can install copying files into Plugin folder inside project or using this repository like git submodule.

## Submodule install

1. initialize submodules in you repo
```sh
$ git submodule init
```
2. add this plugin in submodule list
```sh
$ git submodule add https://github.com/cassiopaixao/Grand.Plugin.Shipping.Rodonaves.git Plugins/Grand.Plugin.Shipping.Rodonaves
```
3. for emotional relief
```sh
$ git submodule update --remote
```

# Settings

After installing the Shipment.Rodonaves plugin, go to Shipping providers' settings*, select Shipment.Rodonaves, and fill the fields below:

| Field | Description |
|-|-|
| Username (API) | Username sent to you by Rodonaves to authenticate via API |
| Password (API) | Password sent to you by Rodonaves to authenticate via API |
| CPF/CNPJ | The tax identification (CPF - 11 numbers, or CNPJ - 14 numbers) associated to your account at Rodonaves |

_\* This setting can be accessed via https://{your_grandnode_domain}/Admin/Shipping/Providers_

# Rodonaves API Reference

[API Reference](https://dev.rte.com.br)

[![N|RTE Rodonaves](logo.jpg)](https://www.rte.com.br)

[![N|GrandNode](https://grandNode.com/Themes/DefaultClean/Content/images/logo.png)](https://grandNode.com)

