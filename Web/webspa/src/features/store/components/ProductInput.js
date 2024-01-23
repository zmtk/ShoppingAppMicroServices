import { Label, FormGroup, Input } from "reactstrap";

const ProductInput = ({ name, label = name.charAt(0).toUpperCase() + name.slice(1), type = "text", value, onChange }) => (
    <FormGroup>
        <Label for={name}>{label}</Label>
        <Input id={name} name={name} type={type} value={value} onChange={onChange} />
    </FormGroup>
);

export default ProductInput;