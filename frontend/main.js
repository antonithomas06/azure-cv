window.addEventListener('DOMContentLoaded', (event)=>{
    getVisitCount();
})
const functionApi= 'http://localhost:7071/api/HttpTrigger1';

const getVisitCount = () => {

    let count = 10;
    fetch(functionApi).then(response =>{
        return response.json()}).then(response=>{
        console.log("Website called function Api");
        count= response.counter;
        document.getElementById("counter").innerText = count;
    }).catch(function(error){
        console.log(error);
    });
    return count;
}