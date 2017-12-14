#!/bin/bash -e
dest=/usr/share/dsts
if [ ! -d $dest ]; then
    echo "INFO: $dsts does not exist. Creating..."
    sudo install -d -o root -g root -m 755 $dest
fi

echo "INFO: Installing WCF libraries to $dest"
lib=bin/Unix.AnyCPU.Debug/System.Private.ServiceModel/System.Private.ServiceModel.dll
sudo install -o root -g root -m 644 ${lib} "${lib::-3}pdb" $dest

libs="System.ServiceModel.Http/System.ServiceModel.Http.dll System.ServiceModel.Duplex/System.ServiceModel.Duplex.dll System.ServiceModel.NetTcp/System.ServiceModel.NetTcp.dll System.ServiceModel.Primitives/System.ServiceModel.Primitives.dll System.ServiceModel.Security/System.ServiceModel.Security.dll"
for lib in $libs; do
    sudo install -o root -g root -m 644 "bin/AnyOS.AnyCPU.Debug/$lib" "bin/AnyOS.AnyCPU.Debug/${lib::-3}pdb" $dest
done
