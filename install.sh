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
lib=bin/Unix.AnyCPU.Debug/System.Private.ServiceModel/System.Private.ServiceModel.dll
$sudo install -o $owner -g $owner -m 644 ${lib} "${lib::-3}pdb" $dest

libs="System.ServiceModel.Http/System.ServiceModel.Http.dll System.ServiceModel.Duplex/System.ServiceModel.Duplex.dll System.ServiceModel.NetTcp/System.ServiceModel.NetTcp.dll System.ServiceModel.Primitives/System.ServiceModel.Primitives.dll System.ServiceModel.Security/System.ServiceModel.Security.dll"
for lib in $libs; do
    $sudo install -o $owner -g $owner -m 644 "bin/AnyOS.AnyCPU.Debug/$lib" "bin/AnyOS.AnyCPU.Debug/${lib::-3}pdb" $dest
done
