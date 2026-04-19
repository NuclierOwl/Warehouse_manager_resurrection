import { dotnet } from './_framework/dotnet.js'

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create()
    .then((dotnetRuntime) => {
        const config = dotnetRuntime.getConfig();
        return dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
    });
