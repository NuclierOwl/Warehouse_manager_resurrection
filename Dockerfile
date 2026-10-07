# syntax=docker/dockerfile:1

# Avalonia 12 desktop app (Proba_Sklada.Desktop) on .NET 10.
# Builds only the Desktop head: the Android/iOS/Browser heads in the solution
# cannot be built on Linux, so we never build the .slnx.

ARG DOTNET_VERSION=10.0
ARG UBUNTU_VERSION=noble

# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-${UBUNTU_VERSION} AS build
WORKDIR /src

# Restore first, against project files only, so the NuGet layer stays cached
# when only .cs/.axaml files change. Directory.Packages.props carries the
# central package versions, so it must be present for restore.
COPY Directory.Packages.props ./
COPY Proba_Sklada/Proba_Sklada.csproj Proba_Sklada/
COPY Proba_Sklada.Desktop/Proba_Sklada.Desktop.csproj Proba_Sklada.Desktop/
RUN dotnet restore Proba_Sklada.Desktop/Proba_Sklada.Desktop.csproj

COPY . .

# ApplicationManifest is cleared because app.manifest is Windows-only.
# Do NOT override OutputType here: it is an MSBuild *global* property, so
# -p:OutputType=Exe would also apply to the referenced Proba_Sklada library
# and make it demand a Main method (CS5001). WinExe is fine on Linux.
RUN dotnet publish Proba_Sklada.Desktop/Proba_Sklada.Desktop.csproj \
        --configuration Release \
        --no-restore \
        --output /app \
        -p:ApplicationManifest=

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/runtime:${DOTNET_VERSION}-${UBUNTU_VERSION} AS runtime

# Avalonia X11 backend needs the X client libraries; libfontconfig/libfreetype
# back SkiaSharp (LiveCharts, QuestPDF), which otherwise renders nothing.
# fontconfig + font packages give the UI a usable fallback typeface.
RUN apt-get update \
    && apt-get install --no-install-recommends --yes \
        libx11-6 \
        libxext6 \
        libxrandr2 \
        libxi6 \
        libxcursor1 \
        libxrender1 \
        libice6 \
        libsm6 \
        libglib2.0-0 \
        libfontconfig1 \
        libfreetype6 \
        fontconfig \
        fonts-dejavu-core \
        fonts-liberation \
        ca-certificates \
    && fc-cache -f \
    && rm -rf /var/lib/apt/lists/*

# Run unprivileged: the X11 socket in /tmp/.X11-unix is world-accessible, so a
# non-root user can still connect to the host display.
RUN useradd --create-home --shell /usr/sbin/nologin --uid 10001 avalonia
ENV HOME=/home/avalonia \
    XDG_RUNTIME_DIR=/tmp/runtime-avalonia \
    DOTNET_NOLOGO=1 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    ConnectionStrings__dbBaza="Server=localhost;Port=5432;Database=bazaV5;Username=user;Password=user"

WORKDIR /app
COPY --from=build --chown=avalonia:avalonia /app ./

# XDG_RUNTIME_DIR must be owned by the running user or Skia/fontconfig warn.
RUN mkdir -p "$XDG_RUNTIME_DIR" && chown avalonia:avalonia "$XDG_RUNTIME_DIR"

USER avalonia
ENV DISPLAY=:0

# No EXPOSE: the app talks to Postgres outbound and draws to the X display.
ENTRYPOINT ["/app/Proba_Sklada.Desktop"]