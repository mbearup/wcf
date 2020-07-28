#!/bin/bash -e
if [ -z "$1" ]; then
  # By default install to /usr/share/dsts, requires sudo
  dest=/usr/share/dsts
  sudo=sudo
  owner=root
else
  # Specify an alternate folder to install without sudo
  dest=$1
  sudo=""
  owner=$USER
fi
if [ ! -d $dest ]; then
    echo "INFO: $dsts does not exist. Creating..."
    $sudo install -d -o $owner -g $owner -m 755 $dest
fi

echo "INFO: Installing WCF libraries to $dest"

prefixes="System.ServiceModel.Http System.ServiceModel.Duplex System.ServiceModel.NetTcp System.ServiceModel.Primitives System.ServiceModel.Security System.Private.ServiceModel"
for prefix in $prefixes; do
    $sudo install -o $owner -g $owner -m 644 "bin/AnyOS.AnyCPU.Debug/$prefix/netstandard/$prefix.dll" $dest
done

dlls="bin/AnyOS.AnyCPU.Debug/corefx/netstandard/System.Net.Http.WinHttpHandler.dll Tools/dotnetcli/sdk/2.1.2/runtimes/unix/lib/netstandard1.3/System.Security.Cryptography.ProtectedData.dll"
for dll in $dlls; do
  $sudo install -o $owner -g $owner -m 644 "${dll}" $dest
done