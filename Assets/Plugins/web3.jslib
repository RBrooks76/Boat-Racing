
mergeInto(LibraryManager.library, {
    Connect: function() {
        if (window.ethereum) {
            web3 = new Web3(window.ethereum);
            // connect popup
            ethereum.enable();

            window.ethereum.on("accountsChanged", function() {
                location.reload();
            });
        } else {
            alert("Check if Metamask is ready.");
        }
    },

    WalletAddress: function () {
        var returnStr;
        try {
            // get address from metamask
            returnStr = web3.currentProvider.selectedAddress
        } catch (e) {
            returnStr = ""
        }

        // Fix Me
        walletAddress = returnStr;
        if (!returnStr)
            returnStr = "";
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },


});