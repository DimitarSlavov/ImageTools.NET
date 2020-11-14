window.SetItemAsync = (key, value) => {
    window.localStorage.setItem(key, value);
};

window.GetItemAsync = (key) => {
    let value = window.localStorage.getItem(key);

    return value;
};